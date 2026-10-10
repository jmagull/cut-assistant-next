using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Cutting;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutProgressActivityTests
{
    private sealed class Clock : TimeProvider
    {
        private long _ticks;
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => _ticks;
        public void Advance(int seconds) => _ticks += TimeSpan.FromSeconds(seconds).Ticks;
    }

    [Fact]
    public void IndexPercentageChangesToIndeterminateCpuPhaseAndDoesNotDuplicateTheProtocol()
    {
        var clock = new Clock();
        var vm = new Mp4BoxProgressViewModel(clock);
        vm.MarkRunning();
        vm.ApplyProgress(new(CutProgressKind.Output, "Indexing, please wait... 78%"));
        vm.ApplyProgress(new(CutProgressKind.Progress, "Indexierung") { Percentage = 78 });
        Assert.False(vm.IsIndeterminate);
        Assert.Equal(78, vm.ProgressValue);
        Assert.Contains("78% dieses Arbeitsschritts", vm.ProgressText);
        Assert.Equal("Indexing, please wait... 78%", vm.ProtocolText);
        clock.Advance(20);
        vm.ApplyProgress(new(CutProgressKind.Progress, "Schnittkante auf CPU neu kodieren"));
        clock.Advance(70);
        vm.RefreshTimings();
        Assert.True(vm.IsIndeterminate);
        Assert.Contains("Laufzeit: 00:01:30", vm.TimingText);
        Assert.Contains("Schritt: 00:01:10", vm.TimingText);
        Assert.Contains("letzte Meldung vor 00:01:10", vm.TimingText);
    }

    [Theory]
    [InlineData(-1d)]
    [InlineData(101d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void InvalidMeasurementsNeverDisplayAPercentage(double value)
    {
        var vm = new Mp4BoxProgressViewModel();
        vm.MarkRunning();
        vm.ApplyProgress(new(CutProgressKind.Progress, "Phase") { Percentage = value });
        Assert.True(vm.IsIndeterminate);
        Assert.Equal(0, vm.ProgressValue);
    }

    [Fact]
    public void CancelRequestAndFailureRemainVisibleDespiteQueuedBackendSteps()
    {
        var vm = new Mp4BoxProgressViewModel();
        vm.MarkRunning();
        vm.MarkCancelRequested();
        vm.ApplyProgress(new(CutProgressKind.Progress, "Alte Indexmeldung") { Percentage = 100 });
        Assert.Equal("Abbruch wird angefordert …", vm.StatusText);
        Assert.True(vm.IsIndeterminate);
        vm.MarkFailed("Werkzeugfehler");
        vm.ApplyProgress(new(CutProgressKind.Progress, "Alte CPU-Meldung"));
        Assert.Contains("Werkzeugfehler", vm.StatusText);
        Assert.False(vm.IsIndeterminate);
        Assert.Equal("Fehlgeschlagen", vm.ProgressText);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CompletionFreezesElapsedTimeAndIgnoresQueuedPhaseUpdates(bool cancelled)
    {
        var clock = new Clock();
        var vm = new Mp4BoxProgressViewModel(clock);
        vm.MarkRunning();
        clock.Advance(35);
        if (cancelled) vm.MarkCancelled(); else vm.MarkSucceeded();
        var status = vm.StatusText;
        clock.Advance(100);
        vm.RefreshTimings();
        vm.ApplyProgress(new(CutProgressKind.Progress, "Veralteter Schritt") { Percentage = 25 });
        Assert.Equal("Laufzeit: 00:00:35", vm.TimingText);
        Assert.Equal(status, vm.StatusText);
        Assert.False(vm.IsIndeterminate);
        Assert.Equal(cancelled ? "Abgebrochen" : "Abgeschlossen", vm.ProgressText);
    }
}
