using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.ViewModels;

internal sealed record FrameLoupeSelection(TimeSpan Position, long Pts, VideoTimeBase TimeBase, int StreamIndex);

internal sealed class FrameLoupeRow(VideoFrameInfo frame, VideoTimeBase timeBase)
{
    public VideoFrameInfo Frame { get; } = frame;
    public int Number => Frame.LocalIndex + 1;
    public decimal? RawSeconds => (Frame.Pts ?? Frame.BestEffortTimestamp) is long timestamp
        ? timeBase.ToSeconds(timestamp) : null;
    public string TimeText => RawSeconds?.ToString("0.000000", CultureInfo.InvariantCulture) ?? "–";
    public string KeyframeText => Frame.IsKeyFrame switch { true => "Ja", false => "Nein", _ => "–" };
}

internal sealed class FrameLoupeViewModel : INotifyPropertyChanged
{
    private readonly IFrameAnalysisRunner _analysis;
    private readonly IFramePreviewRunner _preview;
    private readonly string _mediaFilePath;
    private readonly int _streamIndex;
    private readonly decimal _sourceStart;
    private readonly decimal _sourceEnd;
    private readonly decimal _initialTarget;
    private readonly TimeSpan _mediaDuration;
    private readonly CutEdgeSide _side;
    private readonly HalvingFrameSearch _search;
    private VideoFrameWindow? _window;
    private decimal _probeStart;
    private decimal _probeEnd;
    private string _errorDetails = "";

    internal FrameLoupeViewModel(
        string mediaFilePath, int streamIndex, TimeSpan mediaDuration,
        TimeSpan cutPosition, CutEdgeSide side, decimal sourceStart,
        int initialSearchFrames, IFrameAnalysisRunner analysis, IFramePreviewRunner preview)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaFilePath);
        ArgumentNullException.ThrowIfNull(analysis);
        ArgumentNullException.ThrowIfNull(preview);
        if (streamIndex < 0 || mediaDuration <= TimeSpan.Zero ||
            cutPosition < TimeSpan.Zero || cutPosition > mediaDuration || !Enum.IsDefined(side))
        {
            throw new ArgumentException("Die Ausgangsdaten für die Frame-Lupe sind ungültig.");
        }

        _mediaFilePath = mediaFilePath;
        _streamIndex = streamIndex;
        _analysis = analysis;
        _preview = preview;
        _sourceStart = sourceStart;
        _mediaDuration = mediaDuration;
        _side = side;
        _sourceEnd = sourceStart + (decimal)mediaDuration.Ticks / TimeSpan.TicksPerSecond;
        _initialTarget = sourceStart + (decimal)cutPosition.Ticks / TimeSpan.TicksPerSecond;
        _search = new HalvingFrameSearch(initialSearchFrames);
        Title = $"Frame-Lupe · {(side == CutEdgeSide.Start ? "Startkante" : "Endkante")}";
        CutPositionText = $"Schnittkante: {cutPosition:hh\\:mm\\:ss\\.fff} · {System.IO.Path.GetFileName(mediaFilePath)}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public string Title { get; }
    public string CutPositionText { get; }
    public IReadOnlyList<FrameLoupeRow> Frames { get; private set; } = [];
    public FrameLoupeRow? SelectedFrame { get; private set; }
    public byte[]? PreviewPngBytes { get; private set; }
    public bool IsBusy { get; private set; }
    public bool CanNavigate => !IsBusy && SelectedFrame is not null;
    public bool CanApply => !IsBusy && PreviewPngBytes is { Length: > 0 } && SelectedCutPosition.HasValue;
    public TimeSpan? SelectedCutPosition
    {
        get
        {
            if (SelectedFrame?.Frame.Pts is not long pts || _window is null)
            {
                return null;
            }

            var seconds = _window.TimeBase.ToSeconds(pts) - _sourceStart;
            if (seconds < 0 || seconds > (decimal)_mediaDuration.Ticks / TimeSpan.TicksPerSecond)
            {
                return null;
            }

            return ToTimeSpan(seconds);
        }
    }
    public string SelectedCutPositionText => CanApply && SelectedCutPosition is TimeSpan position
        ? $"Neue Schnittkante: {position:hh\\:mm\\:ss\\.fff}"
        : "Übernahme erst mit bestätigtem Bild innerhalb der Datei möglich.";
    public string EdgeMeaningText => _side == CutEdgeSide.Start
        ? "Dieses Bild ist das erste Bild des zu entfernenden Bereichs."
        : "Dieses Bild bleibt erhalten; der entfernte Bereich endet davor.";
    public string Status { get; private set; } = "Frameanalyse wird vorbereitet …";
    public string PreviewPlaceholderText => IsBusy ? "Vorschau wird geladen …" : "Keine Bildvorschau verfügbar";
    public int NextSearchStep => _search.NextStep;
    public string SearchLeftText => $"← {_search.NextStep:N0} Frames";
    public string SearchRightText => $"{_search.NextStep:N0} Frames →";
    public string FrameText => SelectedFrame is null ? "Kein Frame gewählt"
        : $"Frame {SelectedFrame.Number:N0} / {Frames.Count:N0} im Analyseabschnitt";
    public string DetailsText => FrameDetailsText +
        (string.IsNullOrEmpty(_errorDetails) ? "" : $"\nFehlerdetails:\n{_errorDetails}");

    private string FrameDetailsText => SelectedFrame is null || _window is null ? ""
        : $"Original-PTS: {SelectedFrame.Frame.Pts?.ToString(CultureInfo.InvariantCulture) ?? "–"}\n" +
          $"Best-Effort-Timestamp: {SelectedFrame.Frame.BestEffortTimestamp?.ToString(CultureInfo.InvariantCulture) ?? "–"}\n" +
          $"Paket-DTS: {SelectedFrame.Frame.PacketDts?.ToString(CultureInfo.InvariantCulture) ?? "–"}\n" +
          $"Zeitbasis: {_window.TimeBase.Numerator}/{_window.TimeBase.Denominator}\n" +
          $"Quellzeit: {SelectedFrame.TimeText} s · Keyframe: {SelectedFrame.KeyframeText} · Bildtyp: {SelectedFrame.Frame.PictureType ?? "–"}\n" +
          $"Abstand zur Ausgangskante: {(SelectedFrame.RawSeconds - _initialTarget)?.ToString("+0.000000;-0.000000;0.000000", CultureInfo.InvariantCulture) ?? "–"} s\n" +
          "Ein absoluter Datei-Frameindex liegt noch nicht vor.";

    public Task InitializeAsync(CancellationToken cancellationToken = default) => RunAsync(async () =>
    {
        _probeStart = Math.Max(_sourceStart, _initialTarget - 2);
        _probeEnd = Math.Min(_sourceEnd, _initialTarget + 2);
        SetWindow(await ProbeAsync(_probeStart, _probeEnd, cancellationToken), _initialTarget);
        await RefreshPreviewAsync(cancellationToken);
    }, cancellationToken);

    public FrameLoupeSelection CreateSelection()
    {
        if (!CanApply || SelectedCutPosition is not TimeSpan position)
        {
            throw new InvalidOperationException("Bitte zuerst einen Frame mit bestätigtem Vorschaubild innerhalb der Datei auswählen.");
        }

        return new FrameLoupeSelection(position, SelectedFrame!.Frame.Pts!.Value, _window!.TimeBase, _streamIndex);
    }

    public Task SelectFrameAsync(FrameLoupeRow row, CancellationToken cancellationToken = default)
    {
        if (IsBusy || !Frames.Contains(row))
        {
            return Task.CompletedTask;
        }

        return RunAsync(async () =>
        {
            SelectedFrame = row;
            await RefreshPreviewAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task StepAsync(int frames, CancellationToken cancellationToken = default) =>
        MoveAsync(frames, false, cancellationToken);

    public Task SearchAsync(int direction, CancellationToken cancellationToken = default)
    {
        if (direction is not (-1 or 1))
        {
            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        return MoveAsync(direction * _search.NextStep, true, cancellationToken);
    }

    public void ResetSearch()
    {
        if (!IsBusy)
        {
            _search.Reset();
            PublishChanges();
        }
    }

    private Task MoveAsync(int count, bool search, CancellationToken cancellationToken)
    {
        if (count == 0 || count is < -100000 or > 100000)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (!CanNavigate)
        {
            return Task.CompletedTask;
        }

        return RunAsync(async () =>
        {
            var anchor = SelectedFrame!.RawSeconds
                ?? throw new InvalidOperationException("Der aktuelle Frame hat keinen verwendbaren Zeitstempel.");

            // Grow the decoded interval until the requested number of real frames is available.
            while (true)
            {
                var currentIndex = IndexOfSelectedFrame();
                var requestedIndex = currentIndex + count;
                if (requestedIndex >= 0 && requestedIndex < Frames.Count)
                {
                    break;
                }

                var span = Math.Max(2, _probeEnd - _probeStart);
                var nextStart = count < 0 ? Math.Max(_sourceStart, _probeStart - span) : _probeStart;
                var nextEnd = count > 0 ? Math.Min(_sourceEnd, _probeEnd + span) : _probeEnd;
                if (nextStart == _probeStart && nextEnd == _probeEnd)
                {
                    break;
                }

                Status = "Analyseabschnitt wird erweitert …";
                PublishChanges();
                var window = await ProbeAsync(nextStart, nextEnd, cancellationToken);
                SetWindow(window, anchor, requireExactAnchor: true);
                _probeStart = nextStart;
                _probeEnd = nextEnd;
            }

            var index = IndexOfSelectedFrame();
            var targetIndex = Math.Clamp(index + count, 0, Frames.Count - 1);
            if (targetIndex == index)
            {
                await RefreshPreviewAsync(cancellationToken);
                Status = "Dateigrenze erreicht. Die Suchweite bleibt erhalten.";
                return;
            }

            SelectedFrame = Frames[targetIndex];
            var previewSucceeded = await RefreshPreviewAsync(cancellationToken);
            if (search && previewSucceeded)
            {
                _search.Advance();
            }

            if (targetIndex - index != count && previewSucceeded)
            {
                Status = $"Dateigrenze erreicht: {Math.Abs(targetIndex - index):N0} Frames gesprungen. Bild und PTS stimmen überein.";
            }
        }, cancellationToken);
    }

    private async Task<VideoFrameWindow> ProbeAsync(decimal start, decimal end, CancellationToken cancellationToken)
    {
        return await _analysis.RunAsync(_mediaFilePath, _streamIndex,
            ToTimeSpan(start), ToTimeSpan(end), cancellationToken);
    }

    private void SetWindow(VideoFrameWindow window, decimal anchor, bool requireExactAnchor = false)
    {
        if (window.StreamIndex != _streamIndex || window.Frames.Count == 0)
        {
            throw new InvalidOperationException("Im angefragten Abschnitt wurden keine passenden Videoframes gefunden.");
        }

        if (_window is not null && window.TimeBase != _window.TimeBase)
        {
            throw new InvalidOperationException("Die Zeitbasis hat sich während der Analyse geändert.");
        }

        var rows = window.Frames.Select(frame => new FrameLoupeRow(frame, window.TimeBase)).ToArray();
        decimal? previousTime = null;
        foreach (var row in rows)
        {
            if (row.RawSeconds is decimal time)
            {
                if (previousTime.HasValue && time <= previousTime.Value)
                {
                    throw new InvalidOperationException(
                        "Die Framezeitstempel sind mehrdeutig oder nicht aufsteigend. Eine sichere Navigation ist hier nicht möglich.");
                }

                previousTime = time;
            }
        }

        var selected = requireExactAnchor
            ? rows.FirstOrDefault(row => row.RawSeconds == anchor)
            : rows.Where(row => row.RawSeconds.HasValue)
                .MinBy(row => Math.Abs(row.RawSeconds!.Value - anchor));
        if (selected is null)
        {
            throw new InvalidOperationException("Das Ausgangsbild konnte im Analyseabschnitt nicht eindeutig wiedergefunden werden.");
        }

        _window = window;
        Frames = rows;
        SelectedFrame = selected;
    }

    private int IndexOfSelectedFrame()
    {
        for (var index = 0; index < Frames.Count; index++)
        {
            if (ReferenceEquals(Frames[index], SelectedFrame))
            {
                return index;
            }
        }

        throw new InvalidOperationException("Die aktuelle Frameauswahl ist nicht mehr gültig.");
    }

    private async Task<bool> RefreshPreviewAsync(CancellationToken cancellationToken)
    {
        if (SelectedFrame?.Frame.Pts is not long pts || _window is null)
        {
            Status = "Original-PTS fehlt. Für diesen Frame gibt es keine bestätigte Bildvorschau.";
            return false;
        }

        var preview = await _preview.RunAsync(_mediaFilePath, _streamIndex,
            _window.TimeBase, pts, cancellationToken);
        if (preview.Pts != pts || preview.TimeBase != _window.TimeBase)
        {
            throw new InvalidOperationException("Das Vorschaubild gehört nicht zum gewählten Quellframe.");
        }

        PreviewPngBytes = preview.PngBytes;
        if (preview.PngBytes.Length == 0)
        {
            throw new InvalidOperationException("Für den gewählten Frame wurde kein Vorschaubild geliefert.");
        }

        Status = "Bild und PTS stimmen überein. Mit „Schnittkante übernehmen“ wird die gewählte Zeit in den Schnittplan eingetragen.";
        return true;
    }

    private async Task RunAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        PreviewPngBytes = null;
        _errorDetails = "";
        Status = "Frame wird geladen …";
        PublishChanges();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await operation();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Status = "Vorgang abgebrochen.";
        }
        catch (Exception exception)
        {
            PreviewPngBytes = null;
            Status = exception.Message.Split('\n')[0].TrimEnd('\r');
            _errorDetails = exception.InnerException?.Message ?? exception.ToString();
        }
        finally
        {
            IsBusy = false;
            PublishChanges();
        }
    }

    public void ReportImageError(Exception exception)
    {
        PreviewPngBytes = null;
        Status = "Das Vorschaubild konnte nicht angezeigt werden.";
        _errorDetails = exception.Message;
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(DetailsText));
        OnPropertyChanged(nameof(CanApply));
        OnPropertyChanged(nameof(SelectedCutPositionText));
    }

    private static TimeSpan ToTimeSpan(decimal seconds) =>
        TimeSpan.FromTicks(decimal.ToInt64(decimal.Round(seconds * TimeSpan.TicksPerSecond)));

    private void PublishChanges()
    {
        foreach (var property in new[] { nameof(Frames), nameof(SelectedFrame), nameof(PreviewPngBytes),
            nameof(IsBusy), nameof(CanNavigate), nameof(CanApply), nameof(SelectedCutPosition), nameof(SelectedCutPositionText),
            nameof(Status), nameof(PreviewPlaceholderText), nameof(NextSearchStep), nameof(SearchLeftText),
            nameof(SearchRightText), nameof(FrameText), nameof(DetailsText) })
        {
            OnPropertyChanged(property);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
