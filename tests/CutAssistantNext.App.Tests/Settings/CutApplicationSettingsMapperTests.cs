using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class CutApplicationSettingsMapperTests
{
    [Fact]
    public void ToCutApplicationInfo_WithConfiguredExecutable_ReturnsInfo()
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

        var info =
            CutApplicationSettingsMapper
                .ToCutApplicationInfo(settings);

        Assert.NotNull(info);
        Assert.Equal(
            settings.Name,
            info.Name);
        Assert.Equal(
            settings.ExecutablePath,
            info.Executable);
        Assert.Equal(
            settings.Version,
            info.Version);
        Assert.Equal(
            settings.Options,
            info.Options);
    }

    [Fact]
    public void ToCutApplicationInfo_WithEmptyExecutable_ReturnsNull()
    {
        var settings =
            new CutApplicationSettings
            {
                Name = "MP4Box",
                ExecutablePath = string.Empty,
                Version = "26.07",
                Options = "-splitx"
            };

        var info =
            CutApplicationSettingsMapper
                .ToCutApplicationInfo(settings);

        Assert.Null(info);
    }

    [Fact]
    public void ToCutApplicationInfo_WithWhitespaceExecutable_ReturnsNull()
    {
        var settings =
            new CutApplicationSettings
            {
                ExecutablePath = "   "
            };

        var info =
            CutApplicationSettingsMapper
                .ToCutApplicationInfo(settings);

        Assert.Null(info);
    }

    [Fact]
    public void ToCutApplicationInfo_WithEmptyOptionalValues_PreservesEmptyValues()
    {
        var settings =
            new CutApplicationSettings
            {
                ExecutablePath =
                    @"D:\Programme\MP4Box.exe"
            };

        var info =
            CutApplicationSettingsMapper
                .ToCutApplicationInfo(settings);

        Assert.NotNull(info);
        Assert.Equal(
            string.Empty,
            info.Name);
        Assert.Equal(
            settings.ExecutablePath,
            info.Executable);
        Assert.Equal(
            string.Empty,
            info.Version);
        Assert.Equal(
            string.Empty,
            info.Options);
    }

    [Fact]
    public void ToCutApplicationInfo_WithNullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                CutApplicationSettingsMapper
                    .ToCutApplicationInfo(null!));
    }
}
