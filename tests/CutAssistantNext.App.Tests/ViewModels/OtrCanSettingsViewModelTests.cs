using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.ViewModels;

public sealed class OtrCanSettingsViewModelTests
{
    private static readonly IReadOnlyList<OtrCanToolCheckResult> Success = [new("OTR-CAN", true, "erreichbar")];

    [Fact]
    public async Task CheckAndSave_AreSeparateAndEditingClearsOldResult()
    {
        var saves = 0;
        using var model = new OtrCanSettingsViewModel(new(), (_, _) => Task.FromResult(Success), _ => { saves++; return Task.CompletedTask; });
        await model.CheckAsync();
        Assert.Contains("OK", model.StatusMessage);
        Assert.Equal(0, saves);
        model.FfmsIndexExecutablePath = Path.Combine(Path.GetTempPath(), "ffmsindex.exe");
        Assert.Empty(model.StatusMessage);
        Assert.True(await model.SaveAsync());
        Assert.Equal(1, saves);
    }

    [Fact]
    public async Task ConcurrentCheckOrSaveIsPreventedAndInputsStayFrozen()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<IReadOnlyList<OtrCanToolCheckResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        using var model = new OtrCanSettingsViewModel(new(), (_, _) => { calls++; entered.SetResult(); return release.Task; }, _ => throw new InvalidOperationException());
        var check = model.CheckAsync();
        await entered.Task;
        Assert.True(model.IsBusy);
        Assert.False(model.CanEdit);
        model.ExecutablePath = "changed while busy";
        Assert.Empty(model.ExecutablePath);
        await model.CheckAsync();
        Assert.False(await model.SaveAsync());
        Assert.Equal(1, calls);
        release.SetResult(Success);
        await check;
        Assert.True(model.CanEdit);
    }

    [Fact]
    public async Task Dispose_CancelsCheckWithoutSavingOrPublishingLateResult()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancelled = false;
        var saves = 0;
        var model = new OtrCanSettingsViewModel(new(), async (_, token) =>
        {
            entered.SetResult();
            try { await Task.Delay(Timeout.Infinite, token); }
            catch (OperationCanceledException) { cancelled = true; throw; }
            return Success;
        }, _ => { saves++; return Task.CompletedTask; });
        var check = model.CheckAsync();
        await entered.Task;
        model.Dispose();
        await check;
        Assert.True(cancelled);
        Assert.Equal(0, saves);
        Assert.False(await model.SaveAsync());
        Assert.DoesNotContain("OK", model.StatusMessage);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SaveFailureKeepsDialogEditableAndDoesNotPretendSuccess(bool denied)
    {
        using var model = new OtrCanSettingsViewModel(new(), (_, _) => Task.FromResult(Success),
            _ => Task.FromException(denied
                ? new UnauthorizedAccessException("cannot write")
                : new IOException("cannot write")));
        Assert.False(await model.SaveAsync());
        Assert.Contains("Speichern fehlgeschlagen", model.StatusMessage);
        Assert.True(model.CanEdit);
        Assert.False(model.IsSaving);
    }

    [Fact]
    public async Task InvalidPathNeverReachesPersistence()
    {
        var saves = 0;
        using var model = new OtrCanSettingsViewModel(new() { ExecutablePath = "relative.exe" },
            (_, _) => Task.FromResult(Success), _ => { saves++; return Task.CompletedTask; });
        Assert.False(await model.SaveAsync());
        Assert.Equal(0, saves);
        Assert.Contains("vollständigen Pfad", model.StatusMessage);
    }
}
