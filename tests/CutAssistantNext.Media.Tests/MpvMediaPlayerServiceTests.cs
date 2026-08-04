using System.Collections.Concurrent;
using System.Threading.Channels;
using CutAssistantNext.Core.Logging;
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
            [
                "time-pos",
                "duration",
                "estimated-frame-count",
                "pause"
            ],
            client.ObservedProperties);

        Assert.Equal(MediaPlayerState.Empty, service.State);
        Assert.Equal(100, service.Volume);
        Assert.Null(service.ErrorMessage);
    }

    [Fact]
    public async Task InitializeAsync_LogsStartAndSuccess()
    {
        var client = new StubLibMpvClient();
        var logger = new RecordingAppLogger();

        await using var service =
            new MpvMediaPlayerService(
                client,
                logger);

        await service.InitializeAsync((nint)123);

        Assert.Contains(
            "libmpv-Initialisierung wurde gestartet.",
            logger.InformationMessages);

        Assert.Contains(
            "libmpv wurde erfolgreich initialisiert.",
            logger.InformationMessages);

        Assert.Empty(logger.Errors);
    }

    [Fact]
    public async Task InitializeAsync_LogsFailureWithException()
    {
        var expectedException =
            new InvalidOperationException(
                "Testfehler bei der Initialisierung.");

        var client = new StubLibMpvClient
        {
            InitializeException = expectedException
        };

        var logger = new RecordingAppLogger();

        await using var service =
            new MpvMediaPlayerService(
                client,
                logger);

        var actualException =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.InitializeAsync((nint)123));

        Assert.Same(
            expectedException,
            actualException);

        var loggedError =
            Assert.Single(logger.Errors);

        Assert.Equal(
            "libmpv konnte nicht initialisiert werden.",
            loggedError.Message);

        Assert.Same(
            expectedException,
            loggedError.Exception);
    }

    [Fact]
    public async Task LoadAsync_LogsStartAndFileLoadedSuccess()
    {
        var mediaFilePath = Path.Combine(
            Path.GetTempPath(),
            $"CutAssistantNext-{Guid.NewGuid():N}.mp4");

        File.WriteAllText(mediaFilePath, string.Empty);

        try
        {
            var client = new StubLibMpvClient();
            var logger = new RecordingAppLogger();

            await using var service =
                new MpvMediaPlayerService(
                    client,
                    logger);

            await service.InitializeAsync((nint)123);
            await service.LoadAsync(mediaFilePath);

            var fullMediaFilePath =
                Path.GetFullPath(mediaFilePath);

            Assert.Contains(
                $"Mediendatei wird in mpv geladen: {fullMediaFilePath}",
                logger.InformationMessages);

            client.Publish(
                new LibMpvEvent(
                    LibMpvEventKind.FileLoaded));

            await WaitUntilAsync(
                () => logger.InformationMessages.Contains(
                    $"Mediendatei wurde erfolgreich in mpv geladen: {fullMediaFilePath}"));

            Assert.Empty(logger.Errors);
        }
        finally
        {
            File.Delete(mediaFilePath);
        }
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
    public async Task Events_UpdateStatePositionDurationFramesAndPause()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        var durationChangedCount = 0;

        var frameChangedCount = 0;

        service.DurationChanged +=
            (_, _) => durationChangedCount++;

        service.FrameChanged +=
            (_, _) => frameChangedCount++;

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
                "estimated-frame-count",
                "3013"));

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
                    TimeSpan.FromSeconds(120.5) &&
                service.FrameNumber == 306 &&
                service.EstimatedFrameCount == 3013);

        Assert.Equal(
            MediaPlayerState.Playing,
            service.State);

        Assert.Equal(
            TimeSpan.FromSeconds(12.25),
            service.Position);

        Assert.Equal(
            TimeSpan.FromSeconds(120.5),
            service.Duration);

        Assert.Equal(306, service.FrameNumber);
        Assert.Equal(3013, service.EstimatedFrameCount);

        Assert.Equal(1, durationChangedCount);
        Assert.Equal(2, frameChangedCount);
    }

    [Fact]
    public async Task FrequentPropertyEvents_DoNotCreateLogFlood()
    {
        var client = new StubLibMpvClient();
        var logger = new RecordingAppLogger();

        await using var service =
            new MpvMediaPlayerService(
                client,
                logger);

        await service.InitializeAsync((nint)123);

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.FileLoaded));

        await WaitUntilAsync(
            () => service.State == MediaPlayerState.Paused);

        logger.InformationMessages.Clear();
        logger.WarningMessages.Clear();
        logger.Errors.Clear();

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "duration",
                "120.5"));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "estimated-frame-count",
                "3013"));

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
                service.Position ==
                    TimeSpan.FromSeconds(12.25) &&
                service.Duration ==
                    TimeSpan.FromSeconds(120.5) &&
                service.EstimatedFrameCount == 3013 &&
                service.State ==
                    MediaPlayerState.Playing);

        Assert.Empty(logger.InformationMessages);
        Assert.Empty(logger.WarningMessages);
        Assert.Empty(logger.Errors);
    }

    [Fact]
    public async Task ControlMethods_ForwardToClient()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        var volumeChangedCount = 0;

        service.VolumeChanged +=
            (_, _) => volumeChangedCount++;

        await service.InitializeAsync((nint)123);

        await service.PauseAsync();

        Assert.Equal(
            MediaPlayerState.Paused,
            service.State);

        await service.PlayAsync();

        Assert.Equal(
            MediaPlayerState.Playing,
            service.State);

        await service.SeekAsync(TimeSpan.FromSeconds(42.5));
        await service.SetVolumeAsync(75);
        await service.StopAsync();

        Assert.Equal(
            [
                ("pause", true),
                ("pause", false)
            ],
            client.BooleanPropertyCalls);

        Assert.Equal(
            [
                ("time-pos", 42.5),
                ("volume", 75)
            ],
            client.DoublePropertyCalls);

        Assert.Equal(75, service.Volume);
        Assert.Equal(1, volumeChangedCount);

        var command = Assert.Single(client.Commands);

        Assert.Equal(["stop"], command);
        Assert.Equal(MediaPlayerState.Empty, service.State);
    }

    [Fact]
    public async Task ControlMethods_LogSelectedActionsWithoutVolumeFlood()
    {
        var client = new StubLibMpvClient();
        var logger = new RecordingAppLogger();

        await using var service =
            new MpvMediaPlayerService(
                client,
                logger);

        await service.InitializeAsync((nint)123);
        await service.PauseAsync();
        await service.PlayAsync();
        await service.SeekAsync(
            TimeSpan.FromSeconds(42.5));
        await service.SetVolumeAsync(75);
        await service.StepFramesAsync(10);
        await service.StopAsync();

        Assert.Contains(
            "Wiedergabe wurde pausiert.",
            logger.InformationMessages);

        Assert.Contains(
            "Wiedergabe wurde gestartet.",
            logger.InformationMessages);

        Assert.Contains(
            "Wiedergabeposition wurde geändert: 00:00:42.500.",
            logger.InformationMessages);

        Assert.Contains(
            "Wiedergabe wurde gestoppt.",
            logger.InformationMessages);

        Assert.DoesNotContain(
            logger.InformationMessages,
            message => message.Contains(
                "Lautstärke",
                StringComparison.Ordinal));

        Assert.DoesNotContain(
            logger.InformationMessages,
            message => message.Contains(
                "Bildnavigation",
                StringComparison.Ordinal));

        Assert.Empty(logger.Errors);
    }

    [Theory]
    [InlineData(-25, 0)]
    [InlineData(125, 100)]
    public async Task SetVolumeAsync_ClampsValueToSupportedRange(
        double volume,
        double expectedVolume)
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);
        await service.SetVolumeAsync(volume);

        Assert.Equal(expectedVolume, service.Volume);

        Assert.Equal(
            [("volume", expectedVolume)],
            client.DoublePropertyCalls);
    }

    [Fact]
    public async Task StepForwardAsync_SendsFrameStepCommand()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        await service.StepForwardAsync();

        var command = Assert.Single(client.Commands);

        Assert.Equal(
            ["frame-step", "1", "mute"],
            command);
    }

    [Fact]
    public async Task StepForwardAsync_TransientPauseEvents_KeepPausedState()
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

        var stateChangedCount = 0;

        service.StateChanged +=
            (_, _) => stateChangedCount++;

        await service.StepForwardAsync();

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "pause",
                "no"));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "pause",
                "yes"));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "time-pos",
                "1"));

        await WaitUntilAsync(
            () => service.Position ==
                TimeSpan.FromSeconds(1));

        Assert.Equal(
            MediaPlayerState.Paused,
            service.State);

        Assert.Equal(0, stateChangedCount);
    }

    [Fact]
    public async Task StepBackwardAsync_SendsNegativeFrameStepCommand()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        await service.StepBackwardAsync();

        var command = Assert.Single(client.Commands);

        Assert.Equal(
            ["frame-step", "-1", "seek"],
            command);
    }

    [Theory]
    [InlineData(10, "10", "mute")]
    [InlineData(-10, "-10", "seek")]
    public async Task StepFramesAsync_SendsExpectedCommand(
        int frameCount,
        string expectedFrameCount,
        string expectedMode)
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);

        await service.StepFramesAsync(frameCount);

        var command = Assert.Single(client.Commands);

        Assert.Equal(
            [
                "frame-step",
                expectedFrameCount,
                expectedMode
            ],
            command);
    }

    [Fact]
    public async Task ErrorEvent_SetsErrorState()
    {
        var client = new StubLibMpvClient();
        var logger = new RecordingAppLogger();

        await using var service =
            new MpvMediaPlayerService(
                client,
                logger);

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

        var loggedError =
            Assert.Single(logger.Errors);

        Assert.Equal(
            "libmpv hat einen Fehler gemeldet: Testfehler von libmpv.",
            loggedError.Message);

        Assert.Null(loggedError.Exception);
    }

    [Fact]
    public async Task FileLoaded_AfterPlayRequest_KeepsPlayingState()
    {
        var client = new StubLibMpvClient();

        await using var service =
            new MpvMediaPlayerService(client);

        await service.InitializeAsync((nint)123);
        await service.PlayAsync();

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.FileLoaded));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "time-pos",
                "5"));

        await WaitUntilAsync(
            () => service.Position ==
                TimeSpan.FromSeconds(5));

        Assert.Equal(
            MediaPlayerState.Playing,
            service.State);
    }

    [Fact]
    public async Task PlaybackRestarted_WhilePaused_KeepsPausedState()
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
                LibMpvEventKind.PlaybackRestarted));

        client.Publish(
            new LibMpvEvent(
                LibMpvEventKind.PropertyChanged,
                "time-pos",
                "15"));

        await WaitUntilAsync(
            () => service.Position ==
                TimeSpan.FromSeconds(15));

        Assert.Equal(
            MediaPlayerState.Paused,
            service.State);
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

    private sealed class RecordingAppLogger
        : IAppLogger
    {
        public ConcurrentQueue<string> InformationMessages { get; } = [];

        public ConcurrentQueue<string> WarningMessages { get; } = [];

        public ConcurrentQueue<(string Message, Exception? Exception)> Errors
        {
            get;
        } = [];

        public void Information(string message)
        {
            InformationMessages.Enqueue(message);
        }

        public void Warning(string message)
        {
            WarningMessages.Enqueue(message);
        }

        public void Error(
            string message,
            Exception? exception = null)
        {
            Errors.Enqueue(
                (message, exception));
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

        public Exception? InitializeException { get; set; }

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

            if (InitializeException is not null)
            {
                throw InitializeException;
            }

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
