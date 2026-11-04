using System.Collections;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Tests.Services;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class CutlistSearchDialogTests
{
    [Fact]
    public void ActualDialogGroupsBothSourcesPreservesServerSortAndHasNoPreselection() => Sta(() =>
    {
        var model = Model();
        var dialog = new CutlistSearchResultsDialog(model);
        dialog.Show();
        try
        {
            Pump();
            dialog.UpdateLayout();
            var grid = (DataGrid)dialog.FindName("ResultsGrid");
            Assert.Null(grid.SelectedItem);
            Assert.False(((Button)dialog.FindName("LoadSelectedButton")).IsEnabled);
            Assert.Equal(2, dialog.ResultsView.Groups!.Count);
            var server = Assert.IsAssignableFrom<CollectionViewGroup>(dialog.ResultsView.Groups[0]);
            var local = Assert.IsAssignableFrom<CollectionViewGroup>(dialog.ResultsView.Groups[1]);
            Assert.Equal("Server", server.Name);
            Assert.Equal("Lokal", local.Name);
            Assert.Equal(new[] { "10", "2" }, server.Items.Cast<CutlistResultRow>().Select(row => row.Server!.Id));
            var chosen = Assert.IsType<CutlistResultRow>(local.Items[0]);
            grid.SelectedItem = chosen;
            dialog.UpdateLayout();
            Assert.Same(chosen, model.SelectedResult);
            Assert.True(((Button)dialog.FindName("LoadSelectedButton")).IsEnabled);
            typeof(CutlistSearchResultsDialog).GetMethod("ResultsGrid_Sorting", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(dialog, [grid, new DataGridSortingEventArgs(grid.Columns[2])]);
            Assert.Same(chosen, grid.SelectedItem);
            Assert.Equal("Server", ((CollectionViewGroup)dialog.ResultsView.Groups[0]).Name);
            Pump();
            Assert.Equal(3, Descendants<DataGridRow>(grid).Count());
            var durationColumn = grid.Columns.Single(column => Equals(column.Header, "Dauer"));
            foreach (var row in Descendants<DataGridRow>(grid))
                Assert.Equal("00:01:20.000", Assert.IsType<TextBlock>(durationColumn.GetCellContent(row)).Text);
            Assert.True(grid.ActualHeight >= 150, $"Table height: {grid.ActualHeight}");
            var proof = Environment.GetEnvironmentVariable("CAN_CUTLIST_UI_PROOF");
            if (!string.IsNullOrWhiteSpace(proof))
            {
                Directory.CreateDirectory(proof);
                Capture(dialog, Path.Combine(proof, "cutlists-1180.png"));
            }
            dialog.Width = 1000; dialog.Height = 540; Pump(); dialog.UpdateLayout();
            Assert.True(grid.ActualHeight >= 150, $"Minimum table height: {grid.ActualHeight}");
            Assert.Equal(3, Descendants<DataGridRow>(grid).Count());
            if (!string.IsNullOrWhiteSpace(proof)) Capture(dialog, Path.Combine(proof, "cutlists-1000.png"));
        }
        finally { dialog.Close(); }
    });

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ActualModalActionsRequireExplicitSelectionAndCanContinueWithoutImport(bool load) => Sta(() =>
    {
        var model = Model();
        var dialog = new CutlistSearchResultsDialog(model);
        dialog.Loaded += (_, _) => dialog.Dispatcher.BeginInvoke(() =>
        {
            if (load)
            {
                model.SelectedResult = model.Results.Single(row => row.Local is not null);
                ((Button)dialog.FindName("LoadSelectedButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else dialog.Close();
        });
        var result = dialog.ShowDialog();
        Assert.Equal(load, result == true);
        Assert.Equal(load, model.SelectedResult is not null);
    });

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            if (child is T match) yield return match;
            foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }

    private static void Capture(Window window, string path)
    {
        var content = (FrameworkElement)window.Content;
        var image = new RenderTargetBitmap((int)(content.ActualWidth + content.Margin.Left + content.Margin.Right),
            (int)(content.ActualHeight + content.Margin.Top + content.Margin.Bottom), 96, 96, PixelFormats.Pbgra32);
        image.Render(window);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(image));
        using var stream = File.Create(path); encoder.Save(stream);
    }

    private static CutlistSearchResultsViewModel Model()
    {
        var model = new CutlistSearchResultsViewModel("Aufnahme.HQ.mp4");
        var local = CutlistResultRow.FromLocal(new LocalCutlistCandidate(
            Path.Combine(Path.GetTempPath(), "Eigene Cutlists", "Aufnahme.HQ.mp4.cutlist"), LocalCutlistDiscoveryTests.Bytes()),
            LocalCutlistDiscoveryTests.Context(Path.Combine(Path.GetTempPath(), "Aufnahme.HQ.mp4")));
        model.SearchAsync(_ => Task.FromResult(new CutlistSourceResult(new[] { "2", "10" }.Select(id =>
                CutlistResultRow.FromServer(new CutlistSearchResult
                {
                    Id = id,
                    CutlistFileName = "Aufnahme.HQ.mp4.cutlist",
                    Author = "Serverautor " + id,
                    Rating = "4.5",
                    RatingCount = "3",
                    RatingByAuthor = "5",
                    Cuts = "3",
                    Duration = "80",
                    DownloadCount = "12",
                    UserComment = "Serverfassung",
                    SuggestedMovieName = "Aufnahme"
                }, "https://example.invalid/")).ToArray(), "2 Treffer")),
            _ => Task.FromResult(new CutlistSourceResult([local], "1 Treffer")), CancellationToken.None).GetAwaiter().GetResult();
        return model;
    }

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
        Assert.True(thread.Join(TimeSpan.FromSeconds(20)), "WPF dialog test did not finish.");
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
