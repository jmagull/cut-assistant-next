using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.State;
using CutAssistantNext.App.ViewModels;
using CutAssistantNext.Core.Editing;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Core.Naming;
using CutAssistantNext.Cutlists.IO;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class EditableMovieNameTests
{
    private static NameTemplateContext Context => new(Name: "Chicago Fire", Season: "13", Episode: "07",
        EpisodeTitle: "Unantastbar", OriginalName: "Chicago_Fire_original.HQ.mp4");
    private static MediaAnalysisResult Analysis => new("mp4", "MP4", 100, TimeSpan.FromSeconds(20),
        [new VideoStreamInfo(0, "h264", null, 320, 180, null, null, 25, null)], []);

    [Theory]
    [InlineData("%Name%")]
    [InlineData("%OriginalName%")]
    [InlineData("%Name% %S%%Staffel%%E%%Folge%%Folgentitel: %")]
    public void ManualSuggestionSurvivesMetadataAndMaskChangesUntilExplicitRegeneration(string template)
    {
        var model = new CutlistGenerationViewModel(new(), new CutNamingState(template, Context));
        model.SuggestedMovieName = "Chicago Fire S13E07 – Unantastbar";
        model.Name = "Anderer Titel";
        model.Season = "14";
        model.Episode = "02";
        model.EpisodeTitle = "Andere Folge";
        model.NameTemplate = "%Name%";
        Assert.Equal("Chicago Fire S13E07 – Unantastbar", model.SuggestedMovieName);
        var state = model.CreateNamingState();
        Assert.True(state.PreserveSuggestedMovieName);
        Assert.Equal(Context.OriginalName, state.NameContext.OriginalName);
        var reopened = new CutlistGenerationViewModel(new(), state);
        reopened.EpisodeTitle = "Weiter bearbeitet";
        Assert.Equal(model.SuggestedMovieName, reopened.SuggestedMovieName);
        reopened.GenerateSuggestedMovieName();
        Assert.Equal("Anderer Titel", reopened.SuggestedMovieName);
        Assert.False(reopened.CreateNamingState().PreserveSuggestedMovieName);
        reopened.SuggestedMovieName = "Noch einmal manuell";
        Assert.Equal("Noch einmal manuell", reopened.CreateNamingState().SuggestedMovieName);
    }

    [Fact]
    public void DefaultMaskRemainsAutomaticUntilFirstManualEdit()
    {
        var model = new CutlistGenerationViewModel(new(), NamingSettings.CreateDefault(), Context);
        model.Name = "Neue Serie";
        Assert.StartsWith("Neue Serie", model.SuggestedMovieName);
        model.SuggestedMovieName = "Eigener Titel";
        model.EpisodeTitle = "Verändert";
        Assert.Equal("Eigener Titel", model.SuggestedMovieName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LoadedSuggestionCanBeEditedAndRoundTripsWithoutChangingSourceOrCuts(bool loaded)
    {
        var state = new CutNamingState("%OriginalName%", Context);
        if (loaded) state = state.UseSuggestedMovieName("Fremder Vorschlag", true);
        var model = new CutlistGenerationViewModel(new(), state) { SelectedRating = 4 };
        model.SuggestedMovieName = "Chicago Fire S13E07 – Unantastbar";
        var plan = new CutPlan(Analysis.Duration!.Value);
        plan.Add(new RemoveSegment(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8)));
        var document = model.CreateDocument(plan, Context.OriginalName!, "1.0.3", Analysis);
        var directory = Path.Combine(Path.GetTempPath(), "can-manual-cutlist-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var file = Path.Combine(directory, "original.mp4.cutlist");
        try
        {
            CutlistFileWriter.WriteSafely(file, document);
            var read = CutlistFileReader.Read(file);
            Assert.Equal(model.SuggestedMovieName, read.Info.SuggestedMovieName);
            Assert.Equal(Context.OriginalName, read.General.ApplyToFile);
            Assert.Equal(document.Cuts.Select(x => (x.Start, x.Duration)),
                read.Cuts.Select(x => (x.Start, x.Duration)));
            var originalBytes = File.ReadAllBytes(file);
            model.SuggestedMovieName = "Spätere Bearbeitung";
            Assert.Equal(originalBytes, File.ReadAllBytes(file));
            Assert.Throws<IOException>(() => CutlistFileWriter.WriteSafely(file, document));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Titel/Untertitel")]
    [InlineData("CON")]
    [InlineData("Titel.")]
    public void InvalidManualSuggestionCanBeTypedButCannotBeSaved(string name)
    {
        var model = new CutlistGenerationViewModel(new(), new CutNamingState("%OriginalName%", Context))
        { SelectedRating = 4, SuggestedMovieName = name };
        Assert.Equal(name, model.SuggestedMovieName);
        var error = Assert.Throws<ArgumentException>(() => model.CreateDocument(
            new CutPlan(TimeSpan.FromSeconds(20)), Context.OriginalName!, "1.0.3", Analysis));
        Assert.Contains("name", error.ParamName!, StringComparison.OrdinalIgnoreCase);
        model.GenerateSuggestedMovieName();
        Assert.Equal(Context.OriginalName, model.SuggestedMovieName);
    }

    [Theory]
    [InlineData("%Name%")]
    [InlineData("%OriginalName%")]
    public void OutputChoiceSurvivesBothMotorDirectionsAndCutlistEditing(string template)
    {
        var state = new CutNamingState(template, Context).UseSuggestedMovieName("Cutlist-Titel", true);
        var first = new CutOutputViewModel(state) { OutputMovieName = "Eigene Schnittausgabe.mp4" };
        var second = new CutOutputViewModel(first.CreateNamingState());
        second.Name = "Neue Metadaten";
        Assert.Equal("Eigene Schnittausgabe.mp4", second.OutputMovieName);
        var cutlist = new CutlistGenerationViewModel(new(), second.CreateNamingState());
        cutlist.SuggestedMovieName = "Separater Cutlist-Titel";
        var third = new CutOutputViewModel(cutlist.CreateNamingState());
        Assert.Equal("Eigene Schnittausgabe.mp4", third.OutputMovieName);
        Assert.Equal("Separater Cutlist-Titel", third.SuggestedMovieName);
        third.GenerateSuggestedMovieName();
        Assert.Equal(third.SuggestedMovieName, third.OutputMovieName);
        Assert.False(third.CreateNamingState().PreserveSuggestedMovieName);
        Assert.Equal(third.OutputMovieName, new CutOutputViewModel(third.CreateNamingState()).OutputMovieName);
        Assert.Equal("Eigene Schnittausgabe.mp4", OutputFileNameBuilder.BuildExactMovieFileName(first.OutputMovieName));
    }

    [Fact]
    public void ActualCutlistDialogBindsEditableSuggestionInBothDirections() => Sta(() =>
    {
        var directory = Path.Combine(Path.GetTempPath(), "can-name-dialog-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var model = new CutlistGenerationViewModel(new(), new CutNamingState("%OriginalName%", Context));
        var dialog = new CutlistGenerationDialog(model, new WindowSettingsStore(Path.Combine(directory, "window.json")));
        try
        {
            dialog.Show();
            dialog.UpdateLayout();
            var box = Assert.IsType<TextBox>(dialog.FindName("SuggestedMovieNameTextBox"));
            Assert.False(box.IsReadOnly);
            Assert.True(box.IsVisible);
            Assert.True(box.ActualWidth > 100);
            box.Text = "Chicago Fire S13E07 – Unantastbar";
            box.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
            Assert.Equal(box.Text, model.SuggestedMovieName);
            model.EpisodeTitle = "Metadaten geändert";
            Assert.Equal("Chicago Fire S13E07 – Unantastbar", box.Text);
            model.GenerateSuggestedMovieName();
            box.GetBindingExpression(TextBox.TextProperty)!.UpdateTarget();
            Assert.Equal(Context.OriginalName, box.Text);
            Assert.Equal(BindingMode.TwoWay, box.GetBindingExpression(TextBox.TextProperty)!.ParentBinding.Mode);
        }
        finally { dialog.Close(); Directory.Delete(directory, true); }
    });

    private static void Sta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            try { action(); }
            catch (Exception error) { failure = error; }
            finally { Dispatcher.CurrentDispatcher.InvokeShutdown(); }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(20)), "WPF name test did not finish.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
