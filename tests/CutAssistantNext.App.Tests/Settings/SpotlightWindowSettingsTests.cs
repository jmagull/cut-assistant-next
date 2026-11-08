using System.IO;
using System.Windows;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class SpotlightWindowSettingsTests
{
    private static readonly MonitorWorkArea Primary = new(new(0, 0, 1920, 1040), 1);
    private static readonly MonitorWorkArea Left = new(new(-2560, -200, 2560, 1400), 1);
    private static readonly MonitorWorkArea Third = new(new(1920, 0, 1920, 1040), 1);

    [Fact]
    public void UsesExistingSettingsDirectoryWithAnIndependentSpotlightFile()
    {
        Assert.Equal(Path.GetDirectoryName(WindowSettingsStore.GetDefaultSettingsFilePath()),
            Path.GetDirectoryName(SpotlightWindowSettings.GetSettingsFilePath()));
        Assert.Equal("spotlight-window-settings.json", Path.GetFileName(SpotlightWindowSettings.GetSettingsFilePath()));
    }

    [Fact]
    public void FirstOpenCentersOnOwnerAndUsesSpotlightDefaults()
    {
        var placement = Restore(null, [Primary]);
        Assert.Equal(new Rect(300, 60, 1320, 920), placement.Bounds);
        Assert.False(placement.IsMaximized);
    }

    [Fact]
    public void FirstOpenUsesTheOwnersSecondaryMonitor()
    {
        var placement = SpotlightWindowSettings.Restore(null, [Primary, Left, Third],
            Third.Bounds, 1320, 920, 1000, 800);
        Assert.Equal(new Rect(2220, 60, 1320, 920), placement.Bounds);
    }

    [Fact]
    public void PartiallyOffscreenWindowIsFullyRecoverable()
    {
        Assert.Equal(new Rect(820, 0, 1100, 850), Restore(Settings(1800, -100), [Primary]).Bounds);
    }

    [Theory]
    [InlineData(-2300, -100)]
    [InlineData(2100, 70)]
    public void PreservesSecondAndThirdMonitorCoordinates(double x, double y)
    {
        var saved = Settings(x, y);
        Assert.Equal(new Rect(x, y, 1100, 850), Restore(saved, [Primary, Left, Third]).Bounds);
    }

    [Fact]
    public void MissingMonitorMovesWholeWindowToAvailableWorkArea()
    {
        Assert.Equal(new Rect(0, 0, 1100, 850), Restore(Settings(-2300, -100), [Primary]).Bounds);
    }

    [Fact]
    public void ShrinkingResolutionAlsoReducesMinimumSize()
    {
        var small = new MonitorWorkArea(new(0, 40, 800, 560), 1);
        var placement = Restore(Settings(900, 700), [small]);
        Assert.Equal(small.Bounds, placement.Bounds);
        Assert.Equal(800, placement.MinWidth);
        Assert.Equal(560, placement.MinHeight);
    }

    [Theory]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(2)]
    public void ScalingChangePreservesLogicalSizeAndClampsPosition(double scale)
    {
        var monitor = new MonitorWorkArea(new(0, 0, 3840, 2160), scale);
        var placement = Restore(Settings(100, 80), [monitor]);
        Assert.Equal(new Rect(100, 80, 1100 * scale, 850 * scale), placement.Bounds);
    }

    [Theory]
    [InlineData(double.NaN, 10)]
    [InlineData(10, double.PositiveInfinity)]
    [InlineData(double.MaxValue, double.MinValue)]
    public void InvalidCoordinatesFallBackToOwner(double x, double y)
    {
        Assert.Equal(new Rect(410, 95, 1100, 850), Restore(Settings(x, y), [Primary]).Bounds);
    }

    [Fact]
    public void ExtremeFiniteCoordinatesAreClamped()
    {
        Assert.Equal(new Rect(820, 190, 1100, 850), Restore(Settings(100000, 100000), [Primary]).Bounds);
    }

    [Fact]
    public void InvalidSizeAndDpiUseSafeDefaults()
    {
        var placement = Restore(new() { Left = 10, Top = 20, Width = double.NaN, Height = -5, DpiScale = double.PositiveInfinity }, [Primary]);
        Assert.Equal(new Rect(10, 20, 1320, 920), placement.Bounds);
    }

    [Fact]
    public void DiskRoundTripKeepsNormalGeometryAndMaximization()
    {
        var path = Path.GetTempFileName();
        try
        {
            Assert.True(new WindowSettingsStore(path).Save(new()
            {
                Left = -2300,
                Top = -100,
                Width = 1100,
                Height = 850,
                DpiScale = 1,
                IsMaximized = true
            }));
            var placement = Restore(new WindowSettingsStore(path).Load(), [Primary, Left]);
            Assert.Equal(new Rect(-2300, -100, 1100, 850), placement.Bounds);
            Assert.True(placement.IsMaximized);
        }
        finally { File.Delete(path); }
    }

    [Theory]
    [InlineData("{broken")]
    [InlineData("{\"Left\":\"oops\",\"Top\":10}")]
    [InlineData("{\"Left\":1e999,\"Width\":-1}")]
    [InlineData("null")]
    public void CorruptFilesStillProduceVisibleWindow(string json)
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, json);
            Assert.True(Primary.Bounds.Contains(Restore(new WindowSettingsStore(path).Load(), [Primary]).Bounds));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void OlderSizeOnlySettingsRemainReadable()
    {
        var placement = Restore(new() { Width = 1100, Height = 850, IsMaximized = true }, [Primary]);
        Assert.Equal(new Rect(410, 95, 1100, 850), placement.Bounds);
        Assert.True(placement.IsMaximized);
    }

    private static WindowSettings Settings(double x, double y) => new()
    {
        Left = x,
        Top = y,
        Width = 1100,
        Height = 850,
        DpiScale = 1
    };

    private static SpotlightWindowPlacement Restore(WindowSettings? saved, IReadOnlyList<MonitorWorkArea> monitors) =>
        SpotlightWindowSettings.Restore(saved, monitors, Primary.Bounds, 1320, 920, 1000, 800);
}
