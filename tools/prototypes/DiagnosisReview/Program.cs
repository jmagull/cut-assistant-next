global using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Media.Playback;

namespace DiagnosisReview;

internal static class Program
{
    internal static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [STAThread]
    private static int Main(string[] args)
    {
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };
        try
        {
            if (args.Length < 2 || args[0] is not ("--review" or "--native-test" or "--cut-and-native-test"))
                throw new ArgumentException("--review diagnosis.json [checkpoint-index] | --native-test diagnosis.json new-proof-directory | --cut-and-native-test diagnosis.json new-directory mp4box ffprobe");
            if (args[0] == "--native-test") cancellation.CancelAfter(TimeSpan.FromMinutes(4));
            using var reportLock = File.Open(Path.GetFullPath(args[1]), FileMode.Open, FileAccess.Read, FileShare.Read);
            using var text = new StreamReader(reportLock, leaveOpen: true);
            var request = ReviewRequest.Parse(text.ReadToEnd());
            using var sourceLock = File.Open(request.Source.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var outputLock = File.Open(request.Output.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            request.Source.ValidateAsync(cancellation.Token).GetAwaiter().GetResult();
            request.Output.ValidateAsync(cancellation.Token).GetAwaiter().GetResult();
            reportLock.Position = 0;
            var reportHash = Convert.ToHexString(SHA256.HashData(reportLock));
            if (args[0] == "--cut-and-native-test" && args.Length == 5)
            {
                cancellation.CancelAfter(TimeSpan.FromMinutes(20));
                return RunWpf(async app =>
                {
                    var passed = await IntegrationChecks.RunAsync(request, args[1], reportHash, args[2], args[3], args[4], cancellation.Token);
                    app.Shutdown(passed ? 0 : 1);
                });
            }
            if (args[0] == "--native-test" && args.Length == 3)
                return RunWpf(async app =>
                {
                    var passed = await NativeReviewChecks.RunAsync(request, reportHash, args[2], cancellation.Token);
                    app.Shutdown(passed ? 0 : 1);
                });
            if (args[0] == "--review" && args.Length is 2 or 3)
            {
                var index = args.Length == 3 ? int.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture) : 1;
                if (index < 0 || index > request.Candidates.Count + 1) throw new ArgumentOutOfRangeException(nameof(index));
                return RunWpf(async app =>
                {
                    var player = new TransitionReview.RecordingPlayer(new MpvMediaPlayerService());
                    var window = CreateWindow(player, hidden: false);
                    window.Closed += (_, _) => app.Shutdown();
                    window.Show();
                    await window.ReviewAsync(new(request.CreateCutResult()), index);
                });
            }
            throw new ArgumentException("Ungültige Argumente.");
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            if (args.FirstOrDefault() == "--review") MessageBox.Show(error.Message, "Diagnosekontrolle nicht möglich", MessageBoxButton.OK, MessageBoxImage.Error);
            return 1;
        }
    }

    internal static CutReviewWindow CreateWindow(IMediaPlayerService player, bool hidden)
    {
        var window = new CutReviewWindow(player)
        {
            Title = "Furiosa-Diagnose · Drei-Sekunden-Kontrolle",
            ShowActivated = !hidden,
            ShowInTaskbar = !hidden,
            Opacity = hidden ? 0 : 1
        };
        var heading = ((Grid)window.Content).Children.OfType<StackPanel>().First();
        heading.Children.Insert(0, new TextBlock
        {
            Text = "DIAGNOSE · Prüfkandidaten. Mit „Nächste Prüfstelle“ die vier Übergänge kontrollieren. Lautstärke startet bei null.",
            TextWrapping = TextWrapping.Wrap,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 8)
        });
        return window;
    }

    private static int RunWpf(Func<Application, Task> start)
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
        app.Startup += async (_, _) =>
        {
            try { await start(app); }
            catch (Exception error) { Console.Error.WriteLine(error); app.Shutdown(1); }
        };
        return app.Run();
    }

    internal static void SaveNew(string path, object value)
    {
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, value, JsonOptions);
    }
}
