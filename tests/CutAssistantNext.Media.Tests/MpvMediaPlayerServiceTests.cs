using System.Threading.Channels;
using CutAssistantNext.Media.Playback;
using CutAssistantNext.Media.Playback.Interop;

namespace CutAssistantNext.Media.Tests;

public sealed class MpvMediaPlayerServiceTests
{
    [Fact]
    public async Task InitializeAsync_InitializesClientAndObservesProperties()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        Assert.Equal((nint)123, client.VideoWindowHandle);

        Assert.Equal(
            ["time-pos", "duration", "pause"],
            client.ObservedProperties);

        Assert.Equal(MediaPlayerState.Empty, service.State);
        Assert.Null(service.ErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_SendsLoadCommandAndSetsLoading()
    {
        var mediaFilePath = Path.Combine(
            Path.GetTempPath(),
            $"CutAssistantNext-{Guid.NewGuid():N}.mp4");

        File.WriteAllText(mediaFilePath, string.Empty);

        try
        {
            var client = new StubLibMpvClient();

            await using var service =
                new MpvMediaPlayerService(client);

            await service.InitializeAsync((nint)123);
            await service.LoadAsync(mediaFilePath);

            Assert.Equal(
                MediaPlayerState.Loading,
                service.State);

            var command = Assert.Single(client.Commands);

            Assert.Equal("loadfile", command[0]);

            Assert.Equal(
                Path.GetFullPath(mediaFilePath),
                command[1]);

            Assert.Equal("replace", command[2]);
        }
        finally
        {
            File.Delete(mediaFilePath);
        }
    }

    [Fact]
    public async Task Events_UpdateStatePositionDurationAndPause()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.FileLoaded));

        await WaitUntilAsync(
            () => service.State == MediaPlayerState.Paused);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "duration",
                "120.5"));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "time-pos",
                "12.25"));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "pause",
                "no"));

        await WaitUntilAsync(
            () =>
                service.State == MediaPlayerState.Playing &&
                service.Position ==
                    TimeSpan.FromSeconds(12.25) &&
                service.Duration ==
                    TimeSpan.FromSeconds(120.5));

        Assert.Equal(
            MediaPlayerState.Playing,
            service.State);

        Assert.Equal(
            TimeSpan.FromSeconds(12.25),
            service.Position);

        Assert.Equal(
            TimeSpan.FromSeconds(120.5),
            service.Duration);
    }

    [Fact]
    public async Task ControlMethods_ForwardToClient()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        await service.PauseAsync();
        await service.PlayAsync();
        await service.SeekAsync(TimeSpan.FromSeconds(42.5));
        await service.StopAsync();

        Assert.Equal(
            [
                ("pause", true),
                ("pause", false)
            ],
            client.BooleanPropertyCalls);

        Assert.Equal(
            [("time-pos", 42.5)],
            client.DoublePropertyCalls);

        var command = Assert.Single(client.Commands);

        Assert.Equal(["stop"], command);
        Assert.Equal(MediaPlayerState.Empty, service.State);
    }

    [Fact]
    public async Task ErrorEvent_SetsErrorState()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        var errorEventCount = 0;

        service.ErrorOccurred +=
            (_, _) => errorEventCount++;

        await service.InitializeAsync((nint)123);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.Error,
                ErrorMessage: "Testfehler von libmpv."));

        await WaitUntilAsync(
            () => service.State == MediaPlayerState.Error);

        Assert.Equal(
            "Testfehler von libmpv.",
            service.ErrorMessage);

        Assert.Equal(1, errorEventCount);
    }

    [Fact]
    public async Task EndFileEvent_SetsEndedState()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.FileLoaded));

        await WaitUntilAsync(
            () => service.State == MediaPlayerState.Paused);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.EndFile));

        await WaitUntilAsync(
            () => service.State == MediaPlayerState.Ended);

        Assert.Equal(
            MediaPlayerState.Ended,
            service.State);
    }

    private static async Task WaitUntilAsync(
        Func<bool> condition)
    {
        var timeout =
            DateTimeOffset.UtcNow.AddSeconds(2);

        while (!condition())
        {
            if (DateTimeOffset.UtcNow >= timeout)
            {
                throw new TimeoutException(
                    "Der erwartete Zustand wurde nicht rechtzeitig erreicht.");
            }

            await Task.Delay(10);
        }
    }

    private sealed class StubLibMpvClient
        : ILibMpvClient
    {
        private readonly Channel<LibMpvEvent> _events =
            Channel.CreateUnbounded<LibMpvEvent>();

        public nint VideoWindowHandle { get; private set; }

        public List<string> ObservedProperties { get; } = [];

        public List<string[]> Commands { get; } = [];

        public List<(string Name, bool Value)>
            BooleanPropertyCalls { get; } = [];

        public List<(string Name, double Value)>
            DoublePropertyCalls { get; } = [];

        public bool IsDisposed { get; private set; }

        public IAsyncEnumerable<LibMpvEvent> ReadEventsAsync(
            CancellationToken cancellationToken = default)
        {
            return _events.Reader.ReadAllAsync(
                cancellationToken);
        }

        public Task InitializeAsync(
            nint videoWindowHandle,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            VideoWindowHandle = videoWindowHandle;

            return Task.CompletedTask;
        }

        public Task CommandAsync(
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Commands.Add(arguments.ToArray());

            return Task.CompletedTask;
        }

        public Task ObservePropertyAsync(
            string propertyName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ObservedProperties.Add(propertyName);

            return Task.CompletedTask;
        }

        public Task SetBooleanPropertyAsync(
            string propertyName,
            bool value,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            BooleanPropertyCalls.Add(
                (propertyName, value));

            return Task.CompletedTask;
        }

        public Task SetDoublePropertyAsync(
            string propertyName,
            double value,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DoublePropertyCalls.Add(
                (propertyName, value));

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            IsDisposed = true;
            _events.Writer.TryComplete();

            return ValueTask.CompletedTask;
        }

        public void Publish(LibMpvEvent mpvEvent)
        {
            if (!_events.Writer.TryWrite(mpvEvent))
            {
                throw new InvalidOperationException(
                    "Das Testereignis konnte nicht veröffentlicht werden.");
            }
        }
    }
}