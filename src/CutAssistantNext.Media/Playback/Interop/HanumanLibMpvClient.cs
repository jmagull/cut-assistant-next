using System.Globalization;
using System.Threading.Channels;
using HanumanInstitute.LibMpv;
using HanumanInstitute.LibMpv.Core;

namespace CutAssistantNext.Media.Playback.Interop;

internal sealed class HanumanLibMpvClient : ILibMpvClient
{
    private readonly Channel<LibMpvEvent> _events =
        Channel.CreateUnbounded<LibMpvEvent>(
            new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

    private WindowedMpvContext? _context;
    private long _nextObservationId;
    private bool _disposed;

    public IAsyncEnumerable<LibMpvEvent> ReadEventsAsync(
        CancellationToken cancellationToken = default)
    {
        return _events.Reader.ReadAllAsync(cancellationToken);
    }

    public Task InitializeAsync(
        nint videoWindowHandle,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (videoWindowHandle == 0)
        {
            throw new ArgumentException(
                "Das Video-Fensterhandle darf nicht 0 sein.",
                nameof(videoWindowHandle));
        }

        if (_context is not null)
        {
            throw new InvalidOperationException(
                "Der libmpv-Client wurde bereits initialisiert.");
        }

        try
        {
            var context =
                new WindowedMpvContext(videoWindowHandle);

            context.FileLoaded += OnFileLoaded;
            context.PlaybackRestart += OnPlaybackRestarted;
            context.EndFile += OnEndFile;
            context.PropertyChanged += OnPropertyChanged;

            _context = context;

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            PublishError(exception.Message);
            throw;
        }
    }

    public async Task CommandAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (arguments.Count == 0)
        {
            throw new ArgumentException(
                "Mindestens ein mpv-Befehlsargument wird benötigt.",
                nameof(arguments));
        }

        if (arguments.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "mpv-Befehlsargumente dürfen nicht leer sein.",
                nameof(arguments));
        }

        var context = GetContext();

        var options = new MpvCommandOptions
        {
            NoOsd = true,
            WaitForResponse = true,
            ThrowOnError = true,
            ResponseTimeout = 5_000
        };

        try
        {
            var commandArguments = arguments
                .Cast<object?>()
                .ToArray();

            await context
                .CommandAsync<object?>(
                    options,
                    commandArguments)
                .WaitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            PublishError(exception.Message);
            throw;
        }
    }

    public Task ObservePropertyAsync(
        string propertyName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        cancellationToken.ThrowIfCancellationRequested();

        var context = GetContext();

        var requestId = unchecked(
            (ulong)Interlocked.Increment(
                ref _nextObservationId));

        try
        {
            context.ObserveProperty(
                requestId,
                propertyName,
                MpvFormat.String);

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            PublishError(exception.Message);
            throw;
        }
    }

    public async Task SetBooleanPropertyAsync(
        string propertyName,
        bool value,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        await SetPropertyAsync(
            propertyName,
            value,
            cancellationToken);
    }

    public async Task SetDoublePropertyAsync(
        string propertyName,
        double value,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Der Eigenschaftswert muss endlich sein.");
        }

        await SetPropertyAsync(
            propertyName,
            value,
            cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;

        var context = _context;
        _context = null;

        if (context is not null)
        {
            context.FileLoaded -= OnFileLoaded;
            context.PlaybackRestart -= OnPlaybackRestarted;
            context.EndFile -= OnEndFile;
            context.PropertyChanged -= OnPropertyChanged;

            context.Dispose();
        }

        _events.Writer.TryComplete();

        return ValueTask.CompletedTask;
    }

    private async Task SetPropertyAsync<T>(
        string propertyName,
        T value,
        CancellationToken cancellationToken)
    {
        var context = GetContext();

        var options = new MpvAsyncOptions
        {
            WaitForResponse = true,
            ThrowOnError = true,
            ResponseTimeout = 5_000
        };

        try
        {
            await context
                .SetPropertyAsync(
                    propertyName,
                    value,
                    options)
                .WaitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            PublishError(exception.Message);
            throw;
        }
    }

    private WindowedMpvContext GetContext()
    {
        ThrowIfDisposed();

        return _context
            ?? throw new InvalidOperationException(
                "Der libmpv-Client wurde noch nicht initialisiert.");
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    private void OnFileLoaded(
        object? sender,
        EventArgs eventArgs)
    {
        _events.Writer.TryWrite(
            new LibMpvEvent(
                LibMpvEventKind.FileLoaded));
    }

    private void OnPlaybackRestarted(
        object? sender,
        EventArgs eventArgs)
    {
        _events.Writer.TryWrite(
            new LibMpvEvent(
                LibMpvEventKind.PlaybackRestarted));
    }

    private void OnEndFile(
        object? sender,
        MpvEndFileEventArgs eventArgs)
    {
        _events.Writer.TryWrite(
            new LibMpvEvent(
                LibMpvEventKind.EndFile,
                Value: eventArgs.Reason));

        if (eventArgs.Error != 0)
        {
            PublishError(
                $"mpv beendete die Datei mit Fehlercode " +
                $"{eventArgs.Error} ({eventArgs.Reason}).");
        }
    }

    private void OnPropertyChanged(
        object? sender,
        MpvPropertyEventArgs eventArgs)
    {
        _events.Writer.TryWrite(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                eventArgs.Name,
                eventArgs.Data));
    }

    private void PublishError(string errorMessage)
    {
        _events.Writer.TryWrite(
            new LibMpvEvent(
                LibMpvEventKind.Error,
                ErrorMessage: errorMessage));
    }

    private sealed class WindowedMpvContext
        : MpvContextBase
    {
        [ThreadStatic]
        private static nint _pendingWindowHandle;

        public WindowedMpvContext(
            nint videoWindowHandle)
            : base(PrepareWindowHandle(videoWindowHandle))
        {
            _pendingWindowHandle = 0;
        }

        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            if (_pendingWindowHandle == 0)
            {
                throw new InvalidOperationException(
                    "Das Video-Fensterhandle wurde nicht vorbereitet.");
            }

            SetOptionString(
                "wid",
                _pendingWindowHandle
                    .ToInt64()
                    .ToString(CultureInfo.InvariantCulture));
        }

        private static MpvEventLoop PrepareWindowHandle(
            nint videoWindowHandle)
        {
            _pendingWindowHandle = videoWindowHandle;

            return MpvEventLoop.Thread;
        }
    }
}