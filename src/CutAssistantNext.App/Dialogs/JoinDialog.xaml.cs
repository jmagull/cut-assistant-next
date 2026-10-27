using System.IO;
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
    private bool _closed;
    private bool _busy;
    public Mp4JoinRequest? Request { get; private set; }

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
            _media[index] = media;
            (index == 0 ? FirstPath : SecondPath).Text = open.FileName;
            var choices = media.AudioStreams.Select(a => new AudioTrackCopyChoice(a)).ToArray();
            var combo = index == 0 ? FirstAudio : SecondAudio;
            combo.ItemsSource = choices;
            // Only a sole track can be selected automatically. Multiple tracks require an explicit choice.
            combo.SelectedIndex = choices.Length == 1 ? 0 : -1;
            StatusText.Text = "Bitte Reihenfolge und Tonspuren prüfen. Danach kannst du die Ausgabe festlegen.";
        }
        catch (OperationCanceledException) when (_closed) { }
        catch (Exception error) { if (!_closed) StatusText.Text = error.Message; }
        finally { if (!_closed) SetBusy(false); }
    }

    private void Swap(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        (FirstPath.Text, SecondPath.Text) = (SecondPath.Text, FirstPath.Text);
        (_media[0], _media[1]) = (_media[1], _media[0]);
        var firstItems = FirstAudio.ItemsSource; var firstSelected = FirstAudio.SelectedItem;
        var secondItems = SecondAudio.ItemsSource; var secondSelected = SecondAudio.SelectedItem;
        FirstAudio.ItemsSource = secondItems; FirstAudio.SelectedItem = secondSelected;
        SecondAudio.ItemsSource = firstItems; SecondAudio.SelectedItem = firstSelected;
        UpdateSelection();
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateSelection();
    private void UpdateSelection()
    {
        if (AcceptButton is null) return;
        FirstNotice.Text = Notice(0, FirstAudio.SelectedItem as AudioTrackCopyChoice);
        SecondNotice.Text = Notice(1, SecondAudio.SelectedItem as AudioTrackCopyChoice);
        AcceptButton.IsEnabled = !_busy && FirstAudio.SelectedItem is AudioTrackCopyChoice && SecondAudio.SelectedItem is AudioTrackCopyChoice;
    }
    private string Notice(int index, AudioTrackCopyChoice? choice)
    {
        if (choice is null) return "Bitte die gewünschte Tonspur auswählen.";
        var omitted = _media[index]!.AudioStreams.Where(a => a.Index != choice.Stream.Index).Select(a => new AudioTrackCopyChoice(a).Description).ToArray();
        return "Verwendet: " + choice.Description + (omitted.Length == 0 ? "." : ". Entfallen: " + string.Join(", ", omitted) + ".");
    }
    private void SetBusy(bool value)
    { _busy = value; InputsPanel.IsEnabled = !value; UpdateSelection(); }

    private async void Accept(object sender, RoutedEventArgs e)
    {
        if (_busy || FirstAudio.SelectedItem is not AudioTrackCopyChoice first || SecondAudio.SelectedItem is not AudioTrackCopyChoice second) return;
        SetBusy(true);
        try
        {
            StatusText.Text = "Bildformate und ausgewählte Tonspuren werden vorgeprüft …";
            var verifier = new FfprobeStreamCopyVerifier(_ffprobe);
            var a = await verifier.InspectJoinPartAsync(FirstPath.Text, first.Stream.Index, _cancel.Token);
            var b = await verifier.InspectJoinPartAsync(SecondPath.Text, second.Stream.Index, _cancel.Token);
            FfprobeStreamCopyVerifier.ValidateJoinCompatibility(a, b);
            if (_closed) return;
            var save = new SaveFileDialog
            {
                Title = "Zusammengefügten Film als neue Datei speichern",
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
                try { Mp4JoinService.ValidatePaths(new(FirstPath.Text, first.Stream.Index, SecondPath.Text, second.Stream.Index, save.FileName)); }
                catch (Exception error) { args.Cancel = true; MessageBox.Show(this, error.Message, "Klebecenter", MessageBoxButton.OK, MessageBoxImage.Information); }
            };
            if (save.ShowDialog(this) != true) return;
            Request = new(FirstPath.Text, first.Stream.Index, SecondPath.Text, second.Stream.Index, save.FileName);
            DialogResult = true;
        }
        catch (OperationCanceledException) when (_closed) { }
        catch (Exception error) { if (!_closed) StatusText.Text = error.Message; }
        finally { if (!_closed) SetBusy(false); }
    }
}
