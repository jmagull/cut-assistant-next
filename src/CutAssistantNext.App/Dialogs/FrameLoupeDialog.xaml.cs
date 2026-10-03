using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Dialogs;

internal partial class FrameLoupeDialog : Window
{
    private readonly FrameLoupeViewModel _viewModel;
    private readonly Action<FrameLoupeSelection> _applySelection;
    private readonly CancellationTokenSource _lifetime = new();
    private bool _closed;

    internal FrameLoupeDialog(FrameLoupeViewModel viewModel, Action<FrameLoupeSelection> applySelection)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(applySelection);
        _viewModel = viewModel;
        _applySelection = applySelection;
        InitializeComponent();
        DataContext = viewModel;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        MinWidth = Math.Min(MinWidth, Width);
        MinHeight = Math.Min(MinHeight, Height);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e) =>
        await _viewModel.InitializeAsync(_lifetime.Token);

    private void PreviewScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.ViewportHeightChange != 0)
        {
            UpdatePreviewHeight();
        }
    }

    private void FrameDetails_Changed(object sender, RoutedEventArgs e)
    {
        UpdatePreviewHeight();
        ScrollToSelectedFrame();
    }

    private void ScrollToSelectedFrame()
    {
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(() =>
        {
            if (!_closed && FrameDetails.IsExpanded && _viewModel.SelectedFrame is not null)
            {
                FramesGrid.ScrollIntoView(_viewModel.SelectedFrame);
            }
        }));
    }

    private void UpdatePreviewHeight()
    {
        if (PreviewScroll is not null && PreviewBorder is not null &&
            FrameDetails is not null && PreviewScroll.ViewportHeight > 0)
        {
            PreviewBorder.Height = Math.Max(140,
                PreviewScroll.ViewportHeight - (FrameDetails.IsExpanded ? 320 : 46));
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FrameLoupeViewModel.SelectedFrame))
        {
            ScrollToSelectedFrame();
        }

        if (e.PropertyName != nameof(FrameLoupeViewModel.PreviewPngBytes))
        {
            return;
        }

        FrameImage.Source = null;
        if (_viewModel.PreviewPngBytes is not byte[] bytes)
        {
            return;
        }

        try
        {
            using var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            FrameImage.Source = bitmap;
        }
        catch (Exception exception)
        {
            _viewModel.ReportImageError(exception);
        }
    }

    private async void Step_Click(object sender, RoutedEventArgs e)
    {
        var count = int.Parse((string)((Button)sender).Tag, CultureInfo.InvariantCulture);
        if (Math.Abs(count) == 10 && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            count *= 2;
        }

        await _viewModel.StepAsync(count, _lifetime.Token);
    }

    private async void Search_Click(object sender, RoutedEventArgs e) =>
        await _viewModel.SearchAsync(int.Parse((string)((Button)sender).Tag,
            CultureInfo.InvariantCulture), _lifetime.Token);

    private void ResetSearch_Click(object sender, RoutedEventArgs e) => _viewModel.ResetSearch();

    private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not (Key.Left or Key.Right))
        {
            return;
        }

        var modifiers = Keyboard.Modifiers;
        if (modifiers is not (ModifierKeys.None or ModifierKeys.Control or ModifierKeys.Shift))
        {
            return;
        }

        e.Handled = true;
        var direction = e.Key == Key.Left ? -1 : 1;
        if (modifiers == ModifierKeys.Shift)
        {
            await _viewModel.SearchAsync(direction, _lifetime.Token);
        }
        else
        {
            await _viewModel.StepAsync(direction * (modifiers == ModifierKeys.Control ? 20 : 1), _lifetime.Token);
        }
    }

    private async void FramesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FramesGrid.SelectedItem is FrameLoupeRow row && !ReferenceEquals(row, _viewModel.SelectedFrame))
        {
            await _viewModel.SelectFrameAsync(row, _lifetime.Token);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _applySelection(_viewModel.CreateSelection());
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "Schnittkante übernehmen",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _closed = true;
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        _lifetime.Cancel();
        _lifetime.Dispose();
        FrameImage.Source = null;
    }
}
