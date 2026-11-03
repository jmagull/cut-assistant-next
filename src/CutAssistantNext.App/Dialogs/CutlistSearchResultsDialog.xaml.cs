using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

internal partial class CutlistSearchResultsDialog : Window
{
    internal CutlistSearchResultsDialog(CutlistSearchResultsViewModel model)
    {
        Model = model;
        ResultsView = new ListCollectionView(model.Results);
        ResultsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CutlistResultRow.Source)));
        ResultsView.SortDescriptions.Add(new SortDescription(nameof(CutlistResultRow.SourceOrder), ListSortDirection.Ascending));
        ResultsView.SortDescriptions.Add(new SortDescription(nameof(CutlistResultRow.ServerOrder), ListSortDirection.Descending));
        InitializeComponent();
        DataContext = this;
    }

    public CutlistSearchResultsViewModel Model { get; }
    public ListCollectionView ResultsView { get; }
    internal CutlistResultRow? SelectedResult => Model.SelectedResult;

    private void ResultsGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        e.Handled = true;
        var direction = e.Column.SortDirection == ListSortDirection.Ascending
            ? ListSortDirection.Descending : ListSortDirection.Ascending;
        using (ResultsView.DeferRefresh())
        {
            ResultsView.SortDescriptions.Clear();
            ResultsView.SortDescriptions.Add(new SortDescription(nameof(CutlistResultRow.SourceOrder), ListSortDirection.Ascending));
            ResultsView.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, direction));
            ResultsView.SortDescriptions.Add(new SortDescription(nameof(CutlistResultRow.ServerOrder), ListSortDirection.Descending));
        }
        foreach (var column in ResultsGrid.Columns) column.SortDirection = null;
        e.Column.SortDirection = direction;
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        if (Model.CanLoad) DialogResult = true;
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
