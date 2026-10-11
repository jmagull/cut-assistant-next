using System.IO;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class DialogWindowSettingsTests
{
    [Fact]
    public void EachDialogHasItsOwnFileAndCutlistKeepsTheExistingPath()
    {
        var paths = Enum.GetValues<CutDialogKind>().Select(DialogWindowSettings.GetSettingsFilePath).ToArray();
        Assert.Equal(4, paths.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(paths, path =>
        {
            Assert.Equal(Path.GetDirectoryName(WindowSettingsStore.GetDefaultSettingsFilePath()), Path.GetDirectoryName(path));
            Assert.NotEqual(WindowSettingsStore.GetDefaultSettingsFilePath(), path);
        });
        Assert.Equal("cutlist-window-settings.json", Path.GetFileName(DialogWindowSettings.GetSettingsFilePath(CutDialogKind.CutlistGeneration)));
    }

    [Fact]
    public void MissingSettingsPreserveDefaultSize()
    {
        var size = Restore(null);
        Assert.Equal(900, size.Width);
        Assert.Equal(620, size.Height);
        Assert.False(size.IsMaximized);
    }

    [Fact]
    public void SavedSizeAndMaximizationAreRestored()
    {
        var size = Restore(new() { Width = 1250, Height = 900, IsMaximized = true });
        Assert.Equal(1250, size.Width);
        Assert.Equal(900, size.Height);
        Assert.True(size.IsMaximized);
    }

    [Theory]
    [InlineData(double.NaN, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, -10)]
    [InlineData(0, 0)]
    public void InvalidSavedDimensionsUseDefaults(double width, double height)
    {
        var size = Restore(new() { Width = width, Height = height });
        Assert.Equal(900, size.Width);
        Assert.Equal(620, size.Height);
    }

    [Fact]
    public void SizeIsBoundedByMinimumAndAvailableArea()
    {
        var size = Restore(new() { Width = 50000, Height = 10 });
        Assert.Equal(1920, size.Width);
        Assert.Equal(450, size.Height);
    }

    [Fact]
    public void SmallerWorkAreaAlsoReducesTheEffectiveMinimum()
    {
        var size = DialogWindowSettings.Restore(new() { Width = 1400, Height = 1000 },
            900, 620, 700, 450, 640, 400);
        Assert.Equal(640, size.Width);
        Assert.Equal(400, size.Height);
        Assert.Equal(640, size.MinWidth);
        Assert.Equal(400, size.MinHeight);
    }

    [Fact]
    public void InvalidAvailableAreaFallsBackToFiniteDefaults()
    {
        var size = DialogWindowSettings.Restore(null, 900, 620, 700, 450, double.NaN, 0);
        Assert.Equal(900, size.Width);
        Assert.Equal(620, size.Height);
    }

    [Theory]
    [InlineData(double.NaN, 700)]
    [InlineData(900, double.PositiveInfinity)]
    [InlineData(0, 700)]
    public void InvalidCapturedBoundsAreNotSaved(double width, double height)
    {
        Assert.Null(DialogWindowSettings.Capture(width, height, false));
    }

    [Fact]
    public void SeparateStoreInstancesPreservePreferencesAcrossRuns()
    {
        var directory = Path.Combine(Path.GetTempPath(), "can-dialog-size-" + Guid.NewGuid().ToString("N"));
        try
        {
            var progress = Path.Combine(directory, "progress.json");
            var completion = Path.Combine(directory, "completion.json");
            Assert.True(new WindowSettingsStore(progress).Save(DialogWindowSettings.Capture(1250, 900, true)!));
            Assert.True(new WindowSettingsStore(completion).Save(DialogWindowSettings.Capture(800, 700, false)!));
            var restored = Restore(new WindowSettingsStore(progress).Load());
            Assert.Equal(1250, restored.Width);
            Assert.Equal(900, restored.Height);
            Assert.True(restored.IsMaximized);
            Assert.Equal(800, new WindowSettingsStore(completion).Load()!.Width);
        }
        finally
        {
            File.Delete(Path.Combine(directory, "progress.json"));
            File.Delete(Path.Combine(directory, "completion.json"));
            if (Directory.Exists(directory)) Directory.Delete(directory);
        }
    }

    [Fact]
    public void BrokenSettingsFileUsesDefaults()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{broken");
            Assert.Equal(900, Restore(new WindowSettingsStore(path).Load()).Width);
        }
        finally { File.Delete(path); }
    }

    private static DialogWindowSize Restore(WindowSettings? settings) =>
        DialogWindowSettings.Restore(settings, 900, 620, 700, 450, 1920, 1080);
}
