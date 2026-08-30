using System.Windows.Input;
using System.Windows;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

public partial class NamingSettingsDialog : Window
{
    internal NamingSettingsDialog(
        NamingSettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(
            viewModel);

        InitializeComponent();

        DataContext =
            viewModel;
        PreviewMouseLeftButtonDown +=
            NamingSettingsDialog_PreviewMouseLeftButtonDown;
    }

    private const string TemplateElementDataFormat =
        "CutAssistantNext.NameTemplateElement";

    private const string TemplateBlockIndexDataFormat =
        "CutAssistantNext.NameTemplateBlockIndex";

    private bool _templateElementDropHandled;

    private bool _templateElementDragStarted;

    private void NamingSettingsDialog_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        _templateElementDragStarted = false;
        _templateElementDropHandled = false;
    }

    private void TemplateElement_PreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        if (sender is not FrameworkElement element ||
            element.Tag is not string templateElement ||
            string.IsNullOrWhiteSpace(templateElement))
        {
            return;
        }

        if (_templateElementDragStarted)
        {
            return;
        }

        _templateElementDragStarted = true;
        _templateElementDropHandled = false;

        DragDrop.DoDragDrop(
            element,
            new DataObject(
                TemplateElementDataFormat,
                templateElement),
            DragDropEffects.Copy);
    }

    private void NameTemplateTextBox_PreviewDragOver(
        object sender,
        DragEventArgs e)
    {
        e.Effects =
            e.Data.GetDataPresent(
                TemplateElementDataFormat)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void NameTemplateTextBox_Drop(
        object sender,
        DragEventArgs e)
    {
        if (e.Data.GetData(
                TemplateElementDataFormat)
            is not string templateElement)
        {
            return;
        }

        if (DataContext is not NamingSettingsViewModel viewModel)
        {
            return;
        }

        var selectionStart =
            NameTemplateTextBox.GetCharacterIndexFromPoint(
                e.GetPosition(
                    NameTemplateTextBox),
                snapToText: true);

        if (selectionStart < 0)
        {
            selectionStart =
                NameTemplateTextBox.Text.Length;
        }

        var caretPosition =
            viewModel.InsertTemplateElement(
                templateElement,
                selectionStart,
                selectionLength: 0);

        NameTemplateTextBox.Focus();
        NameTemplateTextBox.CaretIndex =
            caretPosition;
        NameTemplateTextBox.SelectionLength = 0;

        e.Handled = true;
    }

    private void NameTemplateBlockListBox_PreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var sourceIndex =
            NameTemplateBlockListBox.SelectedIndex;

        if (sourceIndex < 0)
        {
            return;
        }

        DragDrop.DoDragDrop(
            NameTemplateBlockListBox,
            new DataObject(
                TemplateBlockIndexDataFormat,
                sourceIndex),
            DragDropEffects.Move);
    }

    private void NameTemplateBlockListBox_PreviewDragOver(
        object sender,
        DragEventArgs e)
    {
        if (e.Data.GetDataPresent(
                TemplateBlockIndexDataFormat))
        {
            e.Effects =
                DragDropEffects.Move;

            e.Handled = true;
            return;
        }

        if (e.Data.GetDataPresent(
                TemplateElementDataFormat))
        {
            e.Effects =
                DragDropEffects.Copy;

            e.Handled = true;
            return;
        }

        e.Effects =
            DragDropEffects.None;

        e.Handled = true;
    }

    private void NameTemplateBlockListBox_Drop(
        object sender,
        DragEventArgs e)
    {
        if (DataContext is not NamingSettingsViewModel viewModel)
        {
            return;
        }

        var insertionIndex =
            GetTemplateBlockInsertionIndex(
                e.GetPosition(
                    NameTemplateBlockListBox));

        if (e.Data.GetData(
                TemplateBlockIndexDataFormat)
            is int sourceIndex)
        {
            var newIndex =
                viewModel.MoveTemplateBlock(
                    sourceIndex,
                    insertionIndex);

            if (newIndex < 0)
            {
                return;
            }

            NameTemplateBlockListBox.SelectedIndex =
                newIndex;

            NameTemplateBlockListBox.Focus();

            e.Handled = true;
            return;
        }

        if (e.Data.GetData(
                TemplateElementDataFormat)
            is not string templateElement)
        {
            return;
        }

        if (_templateElementDropHandled)
        {
            e.Handled = true;
            return;
        }

        var insertedIndex =
            viewModel.InsertTemplateBlock(
                templateElement,
                insertionIndex);

        if (insertedIndex < 0)
        {
            return;
        }

        _templateElementDropHandled = true;

        NameTemplateBlockListBox.SelectedIndex =
            insertedIndex;

        NameTemplateBlockListBox.Focus();

        e.Handled = true;
    }

    private int GetTemplateBlockInsertionIndex(
        Point position)
    {
        var candidates =
            new List<(int Index, Rect Bounds)>();

        for (var index = 0;
             index < NameTemplateBlockListBox.Items.Count;
             index++)
        {
            if (NameTemplateBlockListBox
                    .ItemContainerGenerator
                    .ContainerFromIndex(index)
                is not FrameworkElement item)
            {
                continue;
            }

            var origin =
                item.TranslatePoint(
                    new Point(0, 0),
                    NameTemplateBlockListBox);

            candidates.Add(
                (
                    index,
                    new Rect(
                        origin,
                        item.RenderSize)
                ));
        }

        if (candidates.Count == 0)
        {
            return 0;
        }

        var closestRow =
            candidates
                .OrderBy(
                    candidate =>
                        Math.Abs(
                            position.Y -
                            (
                                candidate.Bounds.Top +
                                candidate.Bounds.Height / 2
                            )))
                .First();

        var rowCenter =
            closestRow.Bounds.Top +
            closestRow.Bounds.Height / 2;

        var rowTolerance =
            candidates.Max(
                candidate =>
                    candidate.Bounds.Height) / 2;

        var rowItems =
            candidates
                .Where(
                    candidate =>
                        Math.Abs(
                            (
                                candidate.Bounds.Top +
                                candidate.Bounds.Height / 2
                            ) -
                            rowCenter) <=
                        rowTolerance)
                .OrderBy(
                    candidate =>
                        candidate.Bounds.Left)
                .ToList();

        foreach (var candidate in rowItems)
        {
            var center =
                candidate.Bounds.Left +
                candidate.Bounds.Width / 2;

            if (position.X < center)
            {
                return candidate.Index;
            }
        }

        return rowItems[^1].Index + 1;
    }

    private void NameTemplateBlockListBox_KeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
        {
            return;
        }

        if (DataContext is not NamingSettingsViewModel viewModel)
        {
            return;
        }

        var selectedIndex =
            NameTemplateBlockListBox.SelectedIndex;

        if (!viewModel.RemoveTemplateBlockAt(
                selectedIndex))
        {
            return;
        }

        if (NameTemplateBlockListBox.Items.Count == 0)
        {
            NameTemplateBlockListBox.SelectedIndex = -1;
        }
        else
        {
            NameTemplateBlockListBox.SelectedIndex =
                Math.Min(
                    selectedIndex,
                    NameTemplateBlockListBox.Items.Count - 1);
        }

        e.Handled = true;
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}