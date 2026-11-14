using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;

namespace CutAssistantNext.App.Tests;

[Collection("WpfUi")]
public sealed class CutlistEndFragmentDialogTests
{
    [Theory]
    [InlineData("KeepButton", true, true, false)]
    [InlineData("RemoveButton", true, true, true)]
    [InlineData("RemoveButton", false, true, true)]
    [InlineData("CancelButton", true, false, false)]
    [InlineData("CancelButton", false, false, false)]
    public void OnlyTheExplicitRemovalActionSelectsRemoval(string buttonName, bool isImport, bool accepted, bool removes)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var dialog = new CutlistEndFragmentDialog("Kurzes Endfragment", isImport);
                var cancel = Assert.IsType<Button>(dialog.FindName("CancelButton"));
                var keep = Assert.IsType<Button>(dialog.FindName("KeepButton"));
                Assert.True(cancel.IsDefault);
                Assert.True(cancel.IsCancel);
                Assert.False(dialog.RemoveFragment);
                Assert.Equal(isImport ? Visibility.Visible : Visibility.Collapsed, keep.Visibility);

                var button = Assert.IsType<Button>(dialog.FindName(buttonName));
                dialog.Dispatcher.InvokeAsync(() =>
                {
                    if (button == cancel)
                        dialog.Close();
                    else
                        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }, DispatcherPriority.ApplicationIdle);

                Assert.Equal(accepted, dialog.ShowDialog() == true);
                Assert.Equal(removes, dialog.RemoveFragment);
            }
            catch (Exception error)
            {
                failure = error;
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)));
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
