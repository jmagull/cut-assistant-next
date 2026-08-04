using System.Windows;
using CutAssistantNext.App.Logging;
using CutAssistantNext.Core;

namespace CutAssistantNext.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly FileAppLogger _logger = new();

    protected override void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        var version = typeof(App)
            .Assembly
            .GetName()
            .Version?
            .ToString()
            ?? "unbekannt";

        _logger.Information(
            $"{ApplicationInfo.ProductName} wurde gestartet. " +
            $"Version: {version}.");

        var mainWindow = new MainWindow(_logger);

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(
        ExitEventArgs e)
    {
        _logger.Information(
            $"{ApplicationInfo.ProductName} wurde beendet. " +
            $"Exit-Code: {e.ApplicationExitCode}.");

        base.OnExit(e);
    }
}
