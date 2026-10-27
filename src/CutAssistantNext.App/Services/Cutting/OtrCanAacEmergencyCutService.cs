using System.IO;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Tools;

namespace CutAssistantNext.App.Services.Cutting;

internal sealed record AacEmergencyChoice(AudioStreamInfo Original, AudioStreamInfo Source)
{
    public string Description => $"Spur {Original.Index}: AAC · {Original.Language ?? "Sprache unbekannt"} · " +
        $"{Original.Channels} Kanäle · {Original.SampleRate} Hz" +
        (string.IsNullOrWhiteSpace(Original.Title) ? "" : $" · {Original.Title}");
}

internal sealed record AacEmergencyOffer(MediaAnalysisResult Original, MediaAnalysisResult Source,
    IReadOnlyList<AacEmergencyChoice> Choices)
{
    internal static AacEmergencyOffer? Create(MediaAnalysisResult original, MediaAnalysisResult source)
    {
        if (original.AudioStreams.Count < 2 || original.AudioStreams.Count != source.AudioStreams.Count) return null;
        var choices = original.AudioStreams.Zip(source.AudioStreams)
            .Where(p => p.First.CodecName == "aac" && p.Second.CodecName == "aac" &&
                p.First.SampleRate == p.Second.SampleRate && p.First.Channels == p.Second.Channels &&
                p.First.ChannelLayout == p.Second.ChannelLayout)
            .Select(p => new AacEmergencyChoice(p.First, p.Second)).ToArray();
        return choices.Length == 0 ? null : new(original, source, Array.AsReadOnly(choices));
    }

    internal string Notice(AacEmergencyChoice choice) =>
        $"Notfallausgabe: nur {choice.Description}. Entfallene Tonspuren: " +
        string.Join(", ", Original.AudioStreams.Where(s => s.Index != choice.Original.Index)
            .Select(s => $"Spur {s.Index} ({s.CodecName}, {s.Language ?? "Sprache unbekannt"}, {s.Channels} Kanäle)")) + ".";
}

internal sealed class OtrCanAudioFailureException(
    MediaAnalysisResult original, MediaAnalysisResult source, Exception inner)
    : InvalidOperationException(inner.Message, inner)
{
    internal AacEmergencyOffer? Offer { get; } = AacEmergencyOffer.Create(original, source);
    internal static bool IsAudioFailure(string message) =>
        message.Contains("PCM assembly emitted warnings; cut rejected", StringComparison.Ordinal) ||
        message.Contains("PCM assembly failed:", StringComparison.Ordinal);
}

internal interface IAacEmergencySource
{
    Task<string> PrepareAsync(CutRequest request, AacEmergencyOffer offer, AacEmergencyChoice choice,
        string work, IProgress<CutProgressUpdate>? progress, CancellationToken token);
}

internal sealed class AacEmergencySource : IAacEmergencySource
{
    private readonly SelectedAudioSource _source;

    internal AacEmergencySource(OtrCanToolPaths tools, ICutToolRunner? runner = null,
        Func<string, CancellationToken, Task<MediaAnalysisResult>>? analyze = null,
        Func<string, string, IReadOnlyList<(int, int)>, CancellationToken, Task>? verify = null)
    {
        _source = new SelectedAudioSource(tools.Ffmpeg, tools.Ffprobe, runner, analyze, verify);
    }

    public Task<string> PrepareAsync(CutRequest request, AacEmergencyOffer offer, AacEmergencyChoice choice,
        string work, IProgress<CutProgressUpdate>? progress, CancellationToken token)
    {
        if (!offer.Choices.Contains(choice)) throw new ArgumentException("Die Tonspur wurde nicht angeboten.");
        return _source.PrepareAsync(request.SourceFilePath, offer.Source, choice.Source, work,
            "aac-source.mp4", progress, token);
    }
}

/// <summary>Retries with one AAC stream only after a specific audio failure and explicit caller consent.</summary>
internal sealed class OtrCanAacEmergencyCutService(
    ICutEngine normal, Func<int, ICutEngine> selectedEngine, IAacEmergencySource preparation,
    Func<AacEmergencyOffer, CancellationToken, Task<AacEmergencyChoice?>> confirm) : ICutEngine
{
    public async Task RunAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default) =>
        _ = await RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false);

    public async Task<CutResult> RunWithResultAsync(CutRequest request, IProgress<CutProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try { return await normal.RunWithResultAsync(request, progress, cancellationToken).ConfigureAwait(false); }
        catch (OtrCanAudioFailureException failure) when (failure.Offer is not null)
        {
            var offer = failure.Offer;
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new(CutProgressKind.Status, "Tonverarbeitung fehlgeschlagen – AAC-Notfallauswahl …"));
            var choice = await confirm(offer, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            if (choice is null) throw;
            if (!offer.Choices.Contains(choice)) throw new ArgumentException("Ungültige AAC-Tonspurauswahl.");
            var notice = offer.Notice(choice);
            progress?.Report(new(CutProgressKind.Output, "Ausdrücklich bestätigt: " + notice));
            var parent = Path.GetDirectoryName(request.OutputFilePath)!;
            var name = ".can-aac-" + Guid.NewGuid().ToString("N");
            var work = Path.Combine(parent, name);
            Directory.CreateDirectory(work);
            try
            {
                var prepared = await preparation.PrepareAsync(request, offer, choice, work, progress, cancellationToken)
                    .ConfigureAwait(false);
                var result = await selectedEngine(choice.Original.Index)
                    .RunWithResultAsync(request.WithSourceFilePath(prepared), progress, cancellationToken).ConfigureAwait(false);
                return new(result.OutputFilePath, result.Timeline, result.TimingKind,
                    notice + (string.IsNullOrWhiteSpace(result.Notice) ? "" : " " + result.Notice));
            }
            finally
            {
                try
                {
                    var resolved = Path.GetFullPath(work);
                    if (Path.GetDirectoryName(resolved) != parent || Path.GetFileName(resolved) != name ||
                        new DirectoryInfo(resolved).LinkTarget is not null)
                        throw new IOException("Der AAC-Arbeitsbereich hat sich verändert.");
                    Directory.Delete(resolved, recursive: true);
                }
                catch (Exception error) when (error is IOException or UnauthorizedAccessException)
                {
                    progress?.Report(new(CutProgressKind.Output, $"AAC-Arbeitsbereich konnte nicht gelöscht werden: {error.Message}"));
                }
            }
        }
    }
}
