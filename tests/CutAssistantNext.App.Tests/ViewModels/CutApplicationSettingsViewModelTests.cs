using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class CutApplicationSettingsViewModelTests
{
    [Fact]
    public void Constructor_UsesSettingsValues()
    {
        var settings =
            new CutApplicationSettings
            {
                Name = "MP4Box",
                ExecutablePath =
                    @"C:\Tools\GPAC\MP4Box.exe",
                Version = "26.07",
                Options = "-splitx"
            };

        var viewModel =
            new CutApplicationSettingsViewModel(
                settings);

        Assert.Equal(
            settings.Name,
            viewModel.Name);
        Assert.Equal(
            settings.ExecutablePath,
            viewModel.ExecutablePath);
        Assert.Equal(
            settings.Version,
            viewModel.Version);
        Assert.Equal(
            settings.Options,
            viewModel.Options);
    }

    [Fact]
    public void Constructor_ProvidesBundledMp4BoxPathWithoutPersistingIt()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-mp4box-viewmodel-" +
            Guid.NewGuid().ToString("N"));

        var resolver = new ToolPathResolver(root);

        var viewModel =
            new CutApplicationSettingsViewModel(
                CutApplicationSettings.CreateDefault(),
                resolver);

        Assert.Equal(
            resolver.GetBundledPath(BundledToolKind.Mp4Box),
            viewModel.BundledMp4BoxExecutablePath);

        var savedSettings = viewModel.CreateSettings();

        Assert.Equal(
            string.Empty,
            savedSettings.ExecutablePath);
    }

    [Fact]
    public void CreateSettings_PreservesCurrentValues()
    {
        var viewModel =
            new CutApplicationSettingsViewModel(
                CutApplicationSettings.CreateDefault())
            {
                Name = "MP4Box",
                ExecutablePath =
                    @"D:\Programme\GPAC\MP4Box.exe",
                Version = "26.07",
                Options = "-splitx"
            };

        var settings =
            viewModel.CreateSettings();

        Assert.Equal(
            viewModel.Name,
            settings.Name);
        Assert.Equal(
            viewModel.ExecutablePath,
            settings.ExecutablePath);
        Assert.Equal(
            viewModel.Version,
            settings.Version);
        Assert.Equal(
            viewModel.Options,
            settings.Options);
    }

    [Fact]
    public void PropertyChange_RaisesPropertyChanged()
    {
        var viewModel =
            new CutApplicationSettingsViewModel(
                CutApplicationSettings.CreateDefault());

        string? changedPropertyName = null;

        viewModel.PropertyChanged +=
            (_, e) =>
            {
                changedPropertyName =
                    e.PropertyName;
            };

        viewModel.ExecutablePath =
            @"C:\Tools\MP4Box.exe";

        Assert.Equal(
            nameof(viewModel.ExecutablePath),
            changedPropertyName);
    }

    [Fact]
    public void SettingSameValue_DoesNotRaisePropertyChanged()
    {
        var settings =
            new CutApplicationSettings
            {
                Name = "MP4Box"
            };

        var viewModel =
            new CutApplicationSettingsViewModel(
                settings);

        var eventCount = 0;

        viewModel.PropertyChanged +=
            (_, _) =>
            {
                eventCount++;
            };

        viewModel.Name =
            "MP4Box";

        Assert.Equal(
            0,
            eventCount);
    }
}
