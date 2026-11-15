using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CutAssistantNext.Core.Media;

namespace CutAssistantNext.App.Controls;

public sealed class KeyframeBoundaryVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values is [TimeSpan boundary, VideoKeyframeIndex index] && index.Contains(boundary)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
