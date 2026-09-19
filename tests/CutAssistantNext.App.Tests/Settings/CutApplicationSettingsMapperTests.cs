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
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-mapper-empty-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var settings =
                new CutApplicationSettings
                {
                    Name = "MP4Box",
                    ExecutablePath = string.Empty,
                    Version = "26.07",
                    Options = "-splitx"
                };

            var resolver = new ToolPathResolver(root);

            var info =
                CutApplicationSettingsMapper
                    .ToCutApplicationInfo(settings, resolver);

            Assert.Null(info);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ToCutApplicationInfo_WithWhitespaceExecutable_ReturnsNull()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-mapper-whitespace-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var settings =
                new CutApplicationSettings
                {
                    ExecutablePath = "   "
                };

            var resolver = new ToolPathResolver(root);

            var info =
                CutApplicationSettingsMapper
                    .ToCutApplicationInfo(settings, resolver);

            Assert.Null(info);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
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
    public void ToCutApplicationInfo_WithBundledMp4Box_ReturnsInfo()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "can-mapper-tests-" +
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var resolver = new ToolPathResolver(root);

            var bundledPath =
                resolver.GetBundledPath(
                    BundledToolKind.Mp4Box);

            Directory.CreateDirectory(
                Path.GetDirectoryName(bundledPath)!);

            File.WriteAllText(
                bundledPath,
                string.Empty);

            var settings =
                CutApplicationSettings.CreateDefault();

            var info =
                CutApplicationSettingsMapper
                    .ToCutApplicationInfo(
                        settings,
                        resolver);

            Assert.NotNull(info);

            Assert.Equal(
                "MP4Box",
                info.Name);

            Assert.Equal(
                bundledPath,
                info.Executable);

            Assert.Equal(
                string.Empty,
                info.Version);

            Assert.Equal(
                string.Empty,
                info.Options);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
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
