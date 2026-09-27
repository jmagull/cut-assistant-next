using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class FfmpegSettingsViewModelTests
{
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
