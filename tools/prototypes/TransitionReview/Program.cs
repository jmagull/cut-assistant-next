global using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace TransitionReview;

internal static class Program
{
    internal static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    [STAThread]
    private static int Main(string[] args)
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(15));
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };
        try
        {
            if (args is ["--analyze", var requestPath, var resultPath])
            {
                var request = Read<AnalysisRequest>(requestPath);
                var parent = Path.GetDirectoryName(Path.GetFullPath(requestPath))!;
                string Resolve(string p) => Path.GetFullPath(p, parent);
                var probe = new FfprobeVideoProbe(Resolve(request.FfprobePath));
                var result = new TransitionAnalyzer(probe).AnalyzeAsync(Resolve(request.OutputFilePath),
                    request.SegmentFilePaths.Select(Resolve).ToArray(), cancellation.Token).GetAwaiter().GetResult();
                SaveNew(resultPath, result);
                Console.WriteLine($"PASS: {result.Segments.Count} Segmente, {result.Transitions.Count} Übergänge verifiziert.");
                foreach (var t in result.Transitions) Console.WriteLine($"{t.Index}: {t.Seconds:F6} s; Prüfstart {Math.Max(0, t.Seconds - 3):F6} s");
                return 0;
            }
            if (args is ["--self-test"])
                return SelfTests.RunAsync(cancellation.Token).GetAwaiter().GetResult();
            if (args is ["--review", var resultFile, var indexText])
            {
                var result = Read<AnalysisResult>(resultFile);
                TransitionAnalyzer.ValidateForReviewAsync(result, cancellation.Token).GetAwaiter().GetResult();
                var index = int.Parse(indexText, System.Globalization.CultureInfo.InvariantCulture);
                if (index < 0 || index > result.Transitions.Count + 1) throw new ArgumentOutOfRangeException(nameof(indexText));
                return RunWpf(async app =>
                {
                    var player = new MpvMediaPlayerService();
                    var window = CreateReviewWindow(player, hidden: false);
                    window.Closed += (_, _) => app.Shutdown();
                    window.Show();
                    await window.ReviewAsync(new CutCompletionViewModel(result), index);
                });
            }
            if (args is ["--native-test", var nativeResult, var proofPath, var referencePath])
            {
                var result = Read<AnalysisResult>(nativeResult);
                TransitionAnalyzer.ValidateForReviewAsync(result, cancellation.Token).GetAwaiter().GetResult();
                return RunWpf(async app =>
                {
                    await NativeChecks.RunAsync(result, Read<NativeReference>(referencePath), proofPath, cancellation.Token);
                    app.Shutdown();
                });
            }
            if (args is ["--native-plan-control", var controlPath, var controlProof, var controlReference])
            {
                // Comparison only; never published as measured MP4Box evidence.
                var control = Read<PlanControlRequest>(controlPath);
                var info = new FfprobeVideoProbe(control.FfprobePath).InspectAsync(control.OutputFilePath, cancellation.Token).GetAwaiter().GetResult();
                if (control.TransitionSeconds.Select((t, i) => t <= 0 || t >= info.Duration || i > 0 && t <= control.TransitionSeconds[i - 1]).Any(x => x))
                    throw new InvalidDataException("Ungültige Plan-Kontrollstellen.");
                var controlResult = new AnalysisResult("plan-control-only", Path.GetFullPath(control.OutputFilePath), "",
                    FileStamp.Read(control.OutputFilePath), info.Duration, [], control.TransitionSeconds.Select((t, i) =>
                        new OutputTransition(i + 1, t, 0, "", false, "PlanControlOnly")).ToArray(), []);
                return RunWpf(async app =>
                {
                    await NativeChecks.RunAsync(controlResult, Read<NativeReference>(controlReference), controlProof, cancellation.Token);
                    app.Shutdown();
                });
            }
            Console.Error.WriteLine("--analyze request.json result.json | --self-test | --review result.json checkpoint-index | --native-test result.json proof.json reference.json | --native-plan-control request.json proof.json reference.json");
            return 2;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    internal static CutReviewWindow CreateReviewWindow(IMediaPlayerService player, bool hidden)
    {
        var window = new CutReviewWindow(player)
        {
            Title = "Prototyp – gemessene Schnittübergänge",
            ShowActivated = !hidden,
            ShowInTaskbar = !hidden,
            Opacity = hidden ? 0 : 1
        };
        var grid = (Grid)window.Content;
        var heading = grid.Children.OfType<StackPanel>().First();
        heading.Children.Insert(0, new TextBlock
        {
            Text = "PROTOTYP · Prüfstellen aus der Ausgabe ermittelt. Fehlende Bilder bleiben fehlend.",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8),
            FontWeight = FontWeights.SemiBold
        });
        return window;
    }

    private static int RunWpf(Func<Application, Task> start)
    {
        var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
        application.Startup += async (_, _) =>
        {
            try { await start(application); }
            catch (Exception error) { Console.Error.WriteLine(error); application.Shutdown(1); }
        };
        return application.Run();
    }

    internal static T Read<T>(string path) => JsonSerializer.Deserialize<T>(File.ReadAllText(path), JsonOptions)
        ?? throw new InvalidDataException("Leere JSON-Datei.");

    internal static void SaveNew<T>(string path, T result)
    {
        using var stream = new FileStream(Path.GetFullPath(path), FileMode.CreateNew, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, result, JsonOptions);
    }
}
