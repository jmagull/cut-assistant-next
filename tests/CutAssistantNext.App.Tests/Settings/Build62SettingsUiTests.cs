using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CutAssistantNext.App.Dialogs;
using CutAssistantNext.App.Services;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.Settings;

[Collection("WpfUi")]
public sealed class Build62SettingsUiTests
{
    [Fact]
    public Task StandardColumnSingleResetAndResetAllShowFactoryGestures() => Sta(() =>
    {
        var personal = new PlaybackShortcuts();
        Assert.Null(personal.Assign(PlaybackShortcut.CutStart, new(Key.F8)));
        var dialog = new ShortcutSettingsDialog(personal);
        try
        {
            var rows = (DataGrid)dialog.FindName("Rows");
            foreach (var (name, text) in new[] { ("CutStart", "A"), ("CutEnd", "E"), ("VideoStart", "Umschalt + A"), ("VideoEnd", "Umschalt + E") })
            {
                var row = rows.Items.Cast<object>().Single(r => r.GetType().GetProperty("Action")!.GetValue(r)!.ToString() == name);
                Assert.Equal(text, row.GetType().GetProperty("Standard")!.GetValue(row));
            }
            rows.SelectedIndex = PlaybackShortcuts.Definitions.ToList().FindIndex(d => d.Action == PlaybackShortcut.CutStart);
            typeof(ShortcutSettingsDialog).GetMethod("Reset_Click", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(dialog, [dialog, new RoutedEventArgs()]);
            Assert.Equal(new(Key.A), dialog.Shortcuts[PlaybackShortcut.CutStart]);
            Assert.Null(dialog.Shortcuts.Assign(PlaybackShortcut.VideoEnd, new(Key.F9)));
            typeof(ShortcutSettingsDialog).GetMethod("ResetAll_Click", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(dialog, [dialog, new RoutedEventArgs()]);
            foreach (var definition in PlaybackShortcuts.Definitions) Assert.Equal(definition.Standard, dialog.Shortcuts[definition.Action]);
        }
        finally { dialog.Close(); }
        return Task.CompletedTask;
    });

    [Fact]
    public Task SettingsWindowRefreshesDisplayOnModePathAndButtonChanges() => Sta(async () =>
    {
        var root = Directory.CreateTempSubdirectory("can-info-ui-");
        var reader = new Reader();
        var vm = new VideoFolderSettingsViewModel(new() { CutVideosDirectory = root.FullName, CustomWorkingDirectory = root.FullName }, reader);
        var dialog = new VideoFolderSettingsDialog(vm, new VideoFolderSettingsStore(Path.Combine(root.FullName, "settings.json"))) { ShowActivated = false, ShowInTaskbar = false };
        try
        {
            dialog.Show(); await WaitReady(vm);
            Assert.Equal(root.FullName, ((TextBlock)dialog.FindName("ResolvedWorkingPath")).Text);
            Assert.Contains("GiB", ((TextBlock)dialog.FindName("WorkDriveCapacity")).Text);
            vm.WorkingDirectoryModeIndex = 1; await WaitReady(vm);
            Assert.Equal(Path.TrimEndingDirectorySeparator(Path.GetTempPath()), ((TextBlock)dialog.FindName("ResolvedWorkingPath")).Text);
            vm.WorkingDirectoryModeIndex = 2; vm.CustomWorkingDirectory = Path.Combine(root.FullName, "custom"); await WaitReady(vm);
            Assert.Equal(vm.CustomWorkingDirectory, ((TextBlock)dialog.FindName("ResolvedWorkingPath")).Text);
            var before = reader.Calls;
            ((Button)dialog.FindName("RefreshWorkStorageButton")).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await WaitReady(vm); Assert.Equal(before + 1, reader.Calls);
            dialog.Close();
            before = reader.Calls; vm.CustomWorkingDirectory = root.FullName;
            await Task.Delay(300); Assert.Equal(before, reader.Calls);
        }
        finally { dialog.Close(); root.Delete(true); }
    });

    private static async Task WaitReady(VideoFolderSettingsViewModel vm)
    {
        await Dispatcher.Yield(DispatcherPriority.ContextIdle);
        for (var attempt = 0; vm.StorageInfo.IsBusy && attempt < 300; attempt++) await Task.Delay(10);
        Assert.False(vm.StorageInfo.IsBusy);
        await Dispatcher.Yield(DispatcherPriority.DataBind);
    }

    private sealed class Reader : IWorkingDirectoryInfoReader
    {
        public int Calls;
        public Task<WorkingDriveInfo> ReadDriveAsync(string path, CancellationToken token)
        { Calls++; return Task.FromResult(new WorkingDriveInfo(100L * 1073741824, 25L * 1073741824)); }
        public Task<WorkingFolderSize> ReadFolderAsync(string path, CancellationToken token) => Task.FromResult(new WorkingFolderSize(2L * 1073741824));
    }

    private static Task Sta(Func<Task> action)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
            dispatcher.InvokeAsync(async () =>
            {
                try { await action(); completion.SetResult(); }
                catch (Exception error) { completion.SetException(error); }
                finally { dispatcher.InvokeShutdown(); }
            });
            Dispatcher.Run();
        });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); return completion.Task;
    }
}
