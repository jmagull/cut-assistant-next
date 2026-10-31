using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Joining;
using Microsoft.Win32;

namespace CutAssistantNext.App.Dialogs;

public partial class JoinDialog : Window
{
    private readonly string _ffprobe;
    private readonly CancellationTokenSource _cancel = new();
    private readonly MediaAnalysisResult?[] _media = new MediaAnalysisResult?[2];
    private readonly bool[] _mp4BoxSources = new bool[2];
    private JoinCopyRow[] _rows = [];
    private bool _closed;
    private bool _busy;
    public Mp4JoinRequest? Request { get; private set; }
    public QuickCopyJoinRequest? QuickRequest { get; private set; }

    public JoinDialog(string ffprobe)
    {
        _ffprobe = ffprobe;
        InitializeComponent();
        MaxHeight = Math.Max(MinHeight, SystemParameters.WorkArea.Height - 40);
        Height = Math.Min(Height, MaxHeight);
        Closed += (_, _) => { _closed = true; _cancel.Cancel(); _cancel.Dispose(); };
    }

    private async void PickFirst(object sender, RoutedEventArgs e) => await Pick(0);
    private async void PickSecond(object sender, RoutedEventArgs e) => await Pick(1);

    private async Task Pick(int index)
    {
        if (_busy) return;
        var open = new OpenFileDialog { Title = $"Bereits geschnittenen Filmteil {index + 1} wählen", Filter = "MP4-Dateien (*.mp4)|*.mp4" };
        if (open.ShowDialog(this) != true) return;
        SetBusy(true);
        try
        {
            StatusText.Text = $"Filmteil {index + 1} wird analysiert …";
            var media = await new FfprobeRunner(_ffprobe).RunAsync(open.FileName, _cancel.Token);
            if (_closed) return;
            if (!VideoPreparation.IsMp4(media) || media.VideoStreams.Count != 1 || media.AudioStreams.Count == 0)
                throw new InvalidDataException("Bitte eine MP4-Datei mit einer Videospur und mindestens einer Tonspur wählen.");
            var mp4Box = await Mp4BoxOriginDetector.IsMp4BoxOutputAsync(open.FileName, _cancel.Token);
            if (_closed) return;
            _media[index] = media;
            _mp4BoxSources[index] = mp4Box;
            (index == 0 ? FirstPath : SecondPath).Text = open.FileName;
            var choices = media.AudioStreams.Select(a => new AudioTrackCopyChoice(a)).ToArray();
            var combo = index == 0 ? FirstAudio : SecondAudio;
            combo.ItemsSource = choices;
            combo.SelectedIndex = choices.Length == 1 ? 0 : -1;
            ResetCopyPairs();
            StatusText.Text = "Bitte Reihenfolge und Tonspuren prüfen. Danach das gewünschte Verfahren bewusst starten.";
        }
        catch (OperationCanceledException) when (_closed) { }
        catch (Exception error) { if (!_closed) StatusText.Text = error.Message; }
        finally { if (!_closed) SetBusy(false); }
    }

    private void Swap(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        var pairs = SelectedPairs();
        (FirstPath.Text, SecondPath.Text) = (SecondPath.Text, FirstPath.Text);
        (_media[0], _media[1]) = (_media[1], _media[0]);
        (_mp4BoxSources[0], _mp4BoxSources[1]) = (_mp4BoxSources[1], _mp4BoxSources[0]);
        var firstItems = FirstAudio.ItemsSource; var firstSelected = FirstAudio.SelectedItem;
        var secondItems = SecondAudio.ItemsSource; var secondSelected = SecondAudio.SelectedItem;
        FirstAudio.ItemsSource = secondItems; FirstAudio.SelectedItem = secondSelected;
        SecondAudio.ItemsSource = firstItems; SecondAudio.SelectedItem = firstSelected;
        ResetCopyPairs(pairs.Select(p => new QuickCopyTrackPair(p.SecondAudioIndex, p.FirstAudioIndex)).ToArray());
        UpdateSelection();
    }

    private void ResetCopyPairs(IReadOnlyList<QuickCopyTrackPair>? previous = null)
    {
        var first = _media[0]?.AudioStreams ?? [];
        var second = _media[1]?.AudioStreams ?? [];
        var proposed = previous ?? QuickCopyAudioPairing.Suggest(first, second);
        _rows = first.Select(a =>
        {
            var match = proposed.SingleOrDefault(p => p.FirstAudioIndex == a.Index);
            return new JoinCopyRow(first, second, a.Index, match?.SecondAudioIndex, match is not null,
                () => { CopyConfirmed.IsChecked = false; UpdateSelection(); });
        }).ToArray();
        CopyPairs.ItemsSource = _rows;
        CopyConfirmed.IsChecked = false;
        UpdateSelection();
    }

    private QuickCopyTrackPair[] SelectedPairs() => _rows.Where(r => r.Include && r.First is not null && r.Second is not null)
        .Select(r => new QuickCopyTrackPair(r.First!.Stream.Index, r.Second!.Stream.Index)).ToArray();

    private void SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateSelection();
    private void CopyConfirmationChanged(object sender, RoutedEventArgs e) => UpdateSelection();
    private void UpdateSelection()
    {
        if (AcceptButton is null || QuickButton is null || CopyNotice is null || EnthusiastInputs is null || EnthusiastAvailabilityNotice is null) return;
        FirstNotice.Text = Notice(0, FirstAudio.SelectedItem as AudioTrackCopyChoice);
        SecondNotice.Text = Notice(1, SecondAudio.SelectedItem as AudioTrackCopyChoice);
        var mp4Box = _mp4BoxSources.Any(value => value);
        EnthusiastInputs.IsEnabled = !mp4Box;
        EnthusiastAvailabilityNotice.Text = mp4Box ? Mp4BoxOriginDetector.EnthusiastUnavailableMessage : "";
        EnthusiastAvailabilityNotice.Visibility = mp4Box ? Visibility.Visible : Visibility.Collapsed;
        AcceptButton.ToolTip = mp4Box ? Mp4BoxOriginDetector.EnthusiastUnavailableMessage : null;
        AcceptButton.IsEnabled = !_busy && !mp4Box && FirstAudio.SelectedItem is AudioTrackCopyChoice && SecondAudio.SelectedItem is AudioTrackCopyChoice;
        var pairs = SelectedPairs();
        var valid = pairs.Length != 0 && pairs.Length == _rows.Count(r => r.Include) &&
            pairs.Select(p => p.FirstAudioIndex).Distinct().Count() == pairs.Length && pairs.Select(p => p.SecondAudioIndex).Distinct().Count() == pairs.Length;
        var lines = pairs.Select((p, i) => $"Ausgabespur {i + 1}: Teil 1 Spur {p.FirstAudioIndex} → Teil 2 Spur {p.SecondAudioIndex}.").ToList();
        for (var i = 0; i < 2; i++)
        {
            var kept = pairs.Select(p => i == 0 ? p.FirstAudioIndex : p.SecondAudioIndex).ToHashSet();
            var omitted = (_media[i]?.AudioStreams ?? []).Where(a => !kept.Contains(a.Index)).Select(a => new AudioTrackCopyChoice(a).Description).ToArray();
            lines.Add($"Teil {i + 1} entfallen: " + (omitted.Length == 0 ? "keine Tonspuren." : string.Join("; ", omitted) + "."));
        }
        if (!valid) lines.Add("Bitte mindestens ein vollständiges, eindeutiges Tonspurpaar auswählen. Keine Spur darf doppelt verwendet werden.");
        CopyNotice.Text = string.Join("\n", lines);
        QuickButton.IsEnabled = !_busy && valid && CopyConfirmed.IsChecked == true;
    }

    private string Notice(int index, AudioTrackCopyChoice? choice)
    {
        if (choice is null) return "Bitte die gewünschte Tonspur auswählen.";
        var omitted = _media[index]!.AudioStreams.Where(a => a.Index != choice.Stream.Index).Select(a => new AudioTrackCopyChoice(a).Description).ToArray();
        return "Verwendet: " + choice.Description + (omitted.Length == 0 ? "." : ". Entfallen: " + string.Join(", ", omitted) + ".");
    }
    private void SetBusy(bool value) { _busy = value; InputsPanel.IsEnabled = !value; UpdateSelection(); }
    private async void Accept(object sender, RoutedEventArgs e) => await Accept(false);
    private async void AcceptQuick(object sender, RoutedEventArgs e) => await Accept(true);

    private async Task Accept(bool quick)
    {
        if (_busy || !quick && !AcceptButton.IsEnabled) return;
        var pairs = quick ? SelectedPairs() : FirstAudio.SelectedItem is AudioTrackCopyChoice first && SecondAudio.SelectedItem is AudioTrackCopyChoice second
            ? [new QuickCopyTrackPair(first.Stream.Index, second.Stream.Index)] : Array.Empty<QuickCopyTrackPair>();
        if (pairs.Length == 0 || quick && !QuickButton.IsEnabled) return;
        SetBusy(true);
        try
        {
            StatusText.Text = "Bildformate und ausgewählte Tonspuren werden vorgeprüft …";
            if (quick)
            {
                var probe = new QuickCopyMediaProbe(_ffprobe);
                var a = await probe.InspectAsync(FirstPath.Text, _cancel.Token);
                var b = await probe.InspectAsync(SecondPath.Text, _cancel.Token);
                QuickCopyMediaProbe.ValidateCompatibility(a, b, pairs);
            }
            else
            {
                var verifier = new FfprobeStreamCopyVerifier(_ffprobe);
                var pair = pairs[0];
                var a = await verifier.InspectJoinPartAsync(FirstPath.Text, pair.FirstAudioIndex, _cancel.Token);
                var b = await verifier.InspectJoinPartAsync(SecondPath.Text, pair.SecondAudioIndex, _cancel.Token);
                FfprobeStreamCopyVerifier.ValidateJoinCompatibility(a, b);
            }
            if (_closed) return;
            var save = new SaveFileDialog
            {
                Title = quick ? "QUICK AND COPY als neue MP4 speichern" : "AUDIO-ENTHUSIAST als neue MP4 speichern",
                Filter = "MP4-Datei (*.mp4)|*.mp4",
                DefaultExt = ".mp4",
                AddExtension = true,
                InitialDirectory = Path.GetDirectoryName(FirstPath.Text),
                FileName = Path.GetFileNameWithoutExtension(FirstPath.Text) + " - zusammengefügt.mp4",
                OverwritePrompt = false,
                CheckPathExists = true
            };
            save.FileOk += (_, args) =>
            {
                try { Mp4JoinService.ValidatePaths(new(FirstPath.Text, pairs[0].FirstAudioIndex, SecondPath.Text, pairs[0].SecondAudioIndex, save.FileName)); }
                catch (Exception error) { args.Cancel = true; MessageBox.Show(this, error.Message, "Klebecenter", MessageBoxButton.OK, MessageBoxImage.Information); }
            };
            if (save.ShowDialog(this) != true) return;
            if (quick) QuickRequest = new(FirstPath.Text, SecondPath.Text, pairs, save.FileName, MappingConfirmed: true);
            else Request = new(FirstPath.Text, pairs[0].FirstAudioIndex, SecondPath.Text, pairs[0].SecondAudioIndex, save.FileName);
            DialogResult = true;
        }
        catch (OperationCanceledException) when (_closed) { }
        catch (Exception error) { if (!_closed) StatusText.Text = error.Message; }
        finally { if (!_closed) SetBusy(false); }
    }
}

internal sealed class JoinCopyRow : INotifyPropertyChanged
{
    private readonly Action _changed;
    private AudioTrackCopyChoice? _first, _second;
    private bool _include;
    public IReadOnlyList<AudioTrackCopyChoice> FirstChoices { get; }
    public IReadOnlyList<AudioTrackCopyChoice> SecondChoices { get; }
    public AudioTrackCopyChoice? First { get => _first; set { _first = value; Change(); } }
    public AudioTrackCopyChoice? Second { get => _second; set { _second = value; Change(); } }
    public bool Include { get => _include; set { _include = value; Change(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    internal JoinCopyRow(IReadOnlyList<AudioStreamInfo> first, IReadOnlyList<AudioStreamInfo> second,
        int firstIndex, int? secondIndex, bool include, Action changed)
    {
        FirstChoices = first.Select(a => new AudioTrackCopyChoice(a)).ToArray();
        SecondChoices = second.Select(a => new AudioTrackCopyChoice(a)).ToArray();
        _first = FirstChoices.Single(a => a.Stream.Index == firstIndex);
        _second = SecondChoices.SingleOrDefault(a => a.Stream.Index == secondIndex);
        _include = include; _changed = changed;
    }
    private void Change([CallerMemberName] string? property = null)
    { PropertyChanged?.Invoke(this, new(property)); _changed(); }
}
