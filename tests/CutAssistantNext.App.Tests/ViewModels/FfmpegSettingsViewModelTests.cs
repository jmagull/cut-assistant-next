using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class FfmpegSettingsViewModelTests
{
    [Fact]
    public async Task CheckReportsRecommendationsAndPathChangeClearsStaleResult()
    {
        using var vm = new FfmpegSettingsViewModel(new(), (_, _) => Task.FromResult<IReadOnlyList<FfmpegToolCheckResult>>(
            [new("FFmpeg", true, "Version 8.1.3 unterstützt; empfohlen ab 9.0.2."), new("ffprobe", false, "Version 5.1 ist zu alt.")]));
        var changes = new List<string?>();
        vm.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        await vm.CheckAsync();
        Assert.Contains("FFmpeg: OK", vm.StatusMessage);
        Assert.Contains("9.0.2", vm.StatusMessage);
        Assert.Contains("ffprobe: Version 5.1 ist zu alt", vm.StatusMessage);
        Assert.True(vm.CanEdit);
        Assert.Contains(nameof(vm.IsBusy), changes);
        vm.FfmpegExecutablePath = "new-path.exe";
        Assert.Empty(vm.StatusMessage);
    }

    [Fact]
    public async Task CheckRunsOffCallerThreadAndPreventsConcurrentEditsAndChecks()
    {
        int callerThread = Environment.CurrentManagedThreadId;
        int workerThread = callerThread;
        using var started = new ManualResetEventSlim();
        var finish = new TaskCompletionSource<IReadOnlyList<FfmpegToolCheckResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        using var vm = new FfmpegSettingsViewModel(new() { FfmpegExecutablePath = "original.exe" }, (_, _) =>
        {
            workerThread = Environment.CurrentManagedThreadId;
            Interlocked.Increment(ref calls);
            started.Set();
            return finish.Task;
        });
        var checking = vm.CheckAsync();
        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        Assert.NotEqual(callerThread, workerThread);
        Assert.True(vm.IsBusy);
        Assert.False(vm.CanEdit);
        vm.FfmpegExecutablePath = "changed.exe";
        await vm.CheckAsync();
        Assert.Equal("original.exe", vm.FfmpegExecutablePath);
        Assert.Equal(1, calls);
        finish.SetResult([new("FFmpeg", true, "Version 9.0.2")]);
        await checking;
        Assert.False(vm.IsBusy);
        Assert.True(vm.CanEdit);
    }

    [Fact]
    public async Task ClosingCancelsCheckAndDoesNotPublishLateResults()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var vm = new FfmpegSettingsViewModel(new(), async (_, token) =>
        {
            started.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token);
            return [];
        });
        var checking = vm.CheckAsync();
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        vm.Dispose();
        await checking;
        Assert.False(vm.IsBusy);
        Assert.False(vm.CanEdit);
        Assert.Equal("Versionen werden geprüft …", vm.StatusMessage);
        await vm.CheckAsync();
    }

    [Fact]
    public async Task FailedCheckRestoresEditStateWithReadableMessage()
    {
        using var vm = new FfmpegSettingsViewModel(new(), (_, _) =>
            Task.FromException<IReadOnlyList<FfmpegToolCheckResult>>(new InvalidOperationException("fixture failure")));
        await vm.CheckAsync();
        Assert.True(vm.CanEdit);
        Assert.Contains("fixture failure", vm.StatusMessage);
    }

    [Fact]
    public void Constructor_UsesSettingsValues()
    {
        var settings = new FfmpegSettings
        {
            FfprobeExecutablePath =
                @"C:\Tools\ffmpeg\bin\ffprobe.exe",
            FfmpegExecutablePath =
                @"C:\Tools\ffmpeg\bin\ffmpeg.exe"
        };

        var viewModel =
            new FfmpegSettingsViewModel(
                settings);

        Assert.Equal(
            settings.FfprobeExecutablePath,
            viewModel.FfprobeExecutablePath);

        Assert.Equal(
            settings.FfmpegExecutablePath,
            viewModel.FfmpegExecutablePath);
    }

    [Fact]
    public void CreateSettings_ReturnsCurrentValues()
    {
        var viewModel =
            new FfmpegSettingsViewModel(
                FfmpegSettings.CreateDefault());

        viewModel.FfprobeExecutablePath =
            @"D:\Programme\ffmpeg\ffprobe.exe";

        viewModel.FfmpegExecutablePath =
            @"D:\Programme\ffmpeg\ffmpeg.exe";

        var settings =
            viewModel.CreateSettings();

        Assert.Equal(
            viewModel.FfprobeExecutablePath,
            settings.FfprobeExecutablePath);

        Assert.Equal(
            viewModel.FfmpegExecutablePath,
            settings.FfmpegExecutablePath);
    }

    [Fact]
    public void Constructor_DefaultSettingsKeepToolPathsEmpty()
    {
        var viewModel = new FfmpegSettingsViewModel(
            FfmpegSettings.CreateDefault());

        var savedSettings = viewModel.CreateSettings();

        Assert.Equal(
            string.Empty,
            savedSettings.FfprobeExecutablePath);

        Assert.Equal(
            string.Empty,
            savedSettings.FfmpegExecutablePath);
    }

    [Fact]
    public void ChangingFfprobePath_RaisesPropertyChanged()
    {
        var viewModel =
            new FfmpegSettingsViewModel(
                FfmpegSettings.CreateDefault());

        string? changedProperty = null;

        viewModel.PropertyChanged +=
            (_, e) =>
                changedProperty =
                    e.PropertyName;

        viewModel.FfprobeExecutablePath =
            @"D:\ffmpeg\ffprobe.exe";

        Assert.Equal(
            nameof(viewModel.FfprobeExecutablePath),
            changedProperty);
    }
}
