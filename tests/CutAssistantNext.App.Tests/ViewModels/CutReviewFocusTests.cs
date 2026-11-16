using System.Windows;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.Core.Logging;

namespace CutAssistantNext.App.Tests.ViewModels;

[Collection("WpfUi")]
public sealed partial class CutReviewViewModelTests
{
    [Theory]
    [InlineData("normal")]
    [InlineData("switch-during-close")]
    [InlineData("inactive")]
    [InlineData("shutdown")]
    public async Task ControlPlayerReturnsActivationOnlyAfterAnActiveUserClose(string scenario)
    {
        var finished = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    await VerifyReviewFocusAsync(scenario);
                    finished.SetResult();
                }
                catch (Exception error) { finished.SetException(error); }
                finally { dispatcher.InvokeShutdown(); }
            });
            Dispatcher.Run();
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(20));
    }

    private static async Task ActivateAsync(Window window)
    {
        if (!window.IsActive) window.Activate();
        await Dispatcher.Yield(DispatcherPriority.Input);
        Assert.True(window.IsActive);
    }

    private static async Task VerifyReviewFocusAsync(string scenario)
    {
        var input = new System.Windows.Controls.TextBox();
        var owner = new Window { Width = 250, Height = 150, ShowInTaskbar = false, Content = input };
        var other = new Window { Width = 250, Height = 150, ShowInTaskbar = false };
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var opened = new List<CutReviewWindow>();
        var controller = new CutReviewController(owner, NullAppLogger.Instance, () =>
        {
            var window = new CutReviewWindow(new Player { Disposal = release.Task }) { ShowInTaskbar = false };
            opened.Add(window);
            return window;
        });
        try
        {
            other.Show();
            owner.Show();
            Assert.True(input.Focus());
            await controller.ReviewAsync(Completion(), 0);
            var review = Assert.Single(opened);
            Assert.Same(owner, review.Owner);
            if (!review.IsActive) review.Activate();
            await Dispatcher.Yield(DispatcherPriority.Input);
            Assert.True(review.IsActive, "Test player must have native WPF activation before close.");

            if (scenario == "inactive") await ActivateAsync(other);
            if (scenario == "shutdown")
            {
                var shutdown = controller.CloseAsync();
                release.SetResult();
                await shutdown;
                Assert.False(review.ReturnActivationToOwner);
                return;
            }
            review.Close();
            Assert.True(review.IsClosing);
            if (scenario == "switch-during-close") await ActivateAsync(other);
            release.SetResult();
            await review.CloseSafelyAsync();

            if (scenario == "normal")
            {
                Assert.True(review.ReturnActivationToOwner);
                await Dispatcher.Yield(DispatcherPriority.Input);
                Assert.True(owner.IsActive);
                Assert.True(owner.IsKeyboardFocusWithin);
                await controller.ReviewAsync(Completion(), 0);
                Assert.Equal(2, opened.Count);
                Assert.NotSame(review, opened[1]);
                Assert.Same(owner, opened[1].Owner);
            }
            else
            {
                Assert.False(review.ReturnActivationToOwner);
                Assert.True(other.IsActive);
            }
        }
        finally
        {
            release.TrySetResult();
            await controller.CloseAsync();
            owner.Close();
            other.Close();
        }
    }
}
