using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.Core.Media;
using CutAssistantNext.Media.Analysis;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class JoinDialogAvailabilityTests
{
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void MixedAndMatchedSourcesGateOnlyEnthusiast(bool firstMp4Box, bool secondMp4Box) => OnUiThread(dialog =>
    {
        SelectSources(dialog, firstMp4Box, secondMp4Box);
        var blocked = firstMp4Box || secondMp4Box;
        Assert.Equal(!blocked, Get<Button>(dialog, "AcceptButton").IsEnabled);
        Assert.Equal(!blocked, Get<StackPanel>(dialog, "EnthusiastInputs").IsEnabled);
        var notice = Get<TextBlock>(dialog, "EnthusiastAvailabilityNotice");
        Assert.Equal(blocked ? Visibility.Visible : Visibility.Collapsed, notice.Visibility);
        // Keep the explanation next to the buttons, visible even when the input area scrolls.
        Assert.Equal(2, Grid.GetRow(Assert.IsType<StackPanel>(notice.Parent)));
        if (blocked) Assert.Equal(Mp4BoxOriginDetector.EnthusiastUnavailableMessage, notice.Text);
        Assert.False(Get<Button>(dialog, "QuickButton").IsEnabled);
        Get<CheckBox>(dialog, "CopyConfirmed").IsChecked = true;
        Assert.True(Get<Button>(dialog, "QuickButton").IsEnabled);
        Assert.Null(dialog.Request);
        Assert.Null(dialog.QuickRequest);
    });

    [Fact]
    public void SwappingKeepsTheRestrictionAndReplacingTheMarkedSourceRestoresAvailability() => OnUiThread(dialog =>
    {
        SelectSources(dialog, true, false);
        Get<CheckBox>(dialog, "CopyConfirmed").IsChecked = true;
        Call(dialog, "Swap", null, new RoutedEventArgs());
        Assert.Equal(new[] { false, true }, Field<bool[]>(dialog, "_mp4BoxSources"));
        Assert.False(Get<Button>(dialog, "AcceptButton").IsEnabled);
        Assert.False(Get<Button>(dialog, "QuickButton").IsEnabled);
        SelectSources(dialog, false, false);
        Assert.True(Get<Button>(dialog, "AcceptButton").IsEnabled);
        Assert.True(Get<StackPanel>(dialog, "EnthusiastInputs").IsEnabled);
        Assert.Equal(Visibility.Collapsed, Get<TextBlock>(dialog, "EnthusiastAvailabilityNotice").Visibility);
        Assert.False(Get<Button>(dialog, "QuickButton").IsEnabled);
    });

    private static void SelectSources(JoinDialog dialog, bool firstMp4Box, bool secondMp4Box)
    {
        var media = Field<MediaAnalysisResult?[]>(dialog, "_media");
        var origins = Field<bool[]>(dialog, "_mp4BoxSources");
        for (var index = 0; index < 2; index++)
        {
            media[index] = new("mp4", null, 100, TimeSpan.FromSeconds(10),
                [new(0, "h264", null, 1280, 720, "1:1", "16:9", 25, "progressive")],
                [new(1, "aac", null, 48000, 2, "stereo")]);
            origins[index] = index == 0 ? firstMp4Box : secondMp4Box;
        }
        for (var index = 0; index < 2; index++)
        {
            var combo = Get<ComboBox>(dialog, index == 0 ? "FirstAudio" : "SecondAudio");
            combo.ItemsSource = media[index]!.AudioStreams.Select(a => new AudioTrackCopyChoice(a)).ToArray();
            combo.SelectedIndex = 0;
        }
        Call(dialog, "ResetCopyPairs", (object?)null);
    }

    private static T Get<T>(JoinDialog dialog, string name) => (T)dialog.FindName(name);
    private static T Field<T>(JoinDialog dialog, string name) => (T)typeof(JoinDialog)
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(dialog)!;
    private static void Call(JoinDialog dialog, string name, params object?[] arguments) => typeof(JoinDialog)
        .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(dialog, arguments);

    private static void OnUiThread(Action<JoinDialog> assertion)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            JoinDialog? dialog = null;
            try { dialog = new JoinDialog("unused-ffprobe"); assertion(dialog); }
            catch (Exception error) { failure = error; }
            finally { dialog?.Close(); Dispatcher.CurrentDispatcher.InvokeShutdown(); }
        })
        { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Klebecenter-Dialogtest überschritt das Zeitlimit.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
