using System.Security.AccessControl;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using CutAssistantNext.App.Services.Cutting;
using CutAssistantNext.App.Settings;
using CutAssistantNext.Core.Cutting;
using CutAssistantNext.Media.Analysis;
using CutAssistantNext.Media.Cutting;

// Arguments: isolated output root, synthetic two-AAC input, engine, ffmsindex, ffmpeg, ffprobe, MP4Box.
var root = Path.GetFullPath(args[0]);
Directory.CreateDirectory(root);
var source = Path.GetFullPath(args[1]);
var sourceIdentity = await CutOutputIdentityVerifier.ReadAsync(source);
var keeps = new[] { new KeepSegment(TimeSpan.Zero, TimeSpan.FromSeconds(2)), new KeepSegment(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(2)) };
var proofs = new List<object>();
foreach (var mode in Enum.GetValues<CutWorkingDirectoryMode>())
{
    var destination = Directory.CreateDirectory(Path.Combine(root, mode.ToString()));
    var custom = Directory.CreateDirectory(Path.Combine(root, "custom-" + mode));
    var settings = new VideoFolderSettings { WorkingDirectoryMode = mode, CustomWorkingDirectory = custom.FullName };
    foreach (var native in new[] { false, true })
    {
        var output = Path.Combine(destination.FullName, native ? "otr-can.mp4" : "mp4box.mp4");
        var work = settings.ResolveWorkingDirectory(output);
        var request = new CutRequest(source, output, keeps, 25).WithWorkingDirectory(work);
        var log = new Log();
        ICutEngine engine = native
            ? new OtrCanCutService(new(args[2], args[3], args[4], args[5]))
            : new Mp4BoxCutService(new Mp4BoxRunner(args[6], progress: log), new FfprobeCutOutputAnalyzer(args[5]));
        CutResult result;
        try { result = await engine.RunWithResultAsync(request, log); }
        finally { await File.WriteAllTextAsync(output + ".log", log.ToString()); }
        var analysis = await new FfprobeRunner(args[5]).RunAsync(output);
        Ensure(analysis.AudioStreams.Count == 2 && analysis.AudioStreams.All(a => a.CodecName == "aac"), "AAC tracks changed");
        Ensure(analysis.Duration is { } duration && Math.Abs((duration - TimeSpan.FromSeconds(4)).TotalSeconds) < .2, "Duration changed");
        if (result.Timeline.Identity is { } identity) await CutOutputIdentityVerifier.ValidateAsync(output, identity, true);
        var protocol = log.ToString();
        Ensure(protocol.Contains("Gemessen frei") && protocol.Contains("Geschätzter zusätzlicher") && protocol.Contains(work), "Missing storage diagnostics");
        Ensure(log.Workspaces.All(p => !Directory.Exists(p)), "Workspace not cleaned");
        await File.WriteAllTextAsync(output + ".log", protocol);
        proofs.Add(new { Mode = mode.ToString(), Engine = native ? "otr-can" : "MP4Box", WorkRoot = work, Output = output,
            AudioTracks = 2, Duration = analysis.Duration?.TotalSeconds, Timing = result.TimingKind.ToString(), WorkspacesCleaned = true });
    }
}

foreach (var native in new[] { false, true })
{
    var output = Path.Combine(root, native ? "cancel-otr.mp4" : "cancel-mp4box.mp4");
    await File.WriteAllTextAsync(output, "preserve old target");
    using var cancellation = new CancellationTokenSource();
    var log = new Log(update =>
    {
        if (update.Kind == CutProgressKind.Status && (update.Message.StartsWith("Segment 1") || update.Message.Contains("CPU-Encoding"))) cancellation.Cancel();
    });
    ICutEngine engine = native ? new OtrCanCutService(new(args[2], args[3], args[4], args[5])) : new Mp4BoxCutService(new Mp4BoxRunner(args[6], progress: log));
    try
    {
        await engine.RunAsync(new CutRequest(source, output, keeps, 25, true).WithWorkingDirectory(root), log, cancellation.Token);
        throw new InvalidOperationException("Cancellation was ignored");
    }
    catch (OperationCanceledException) { }
    Ensure(await File.ReadAllTextAsync(output) == "preserve old target", "Target changed after cancellation");
    Ensure(log.Workspaces.All(p => !Directory.Exists(p)), "Canceled work not cleaned");
    await File.WriteAllTextAsync(output + ".log", log.ToString());
    proofs.Add(new { Engine = native ? "otr-can" : "MP4Box", ControlledCancellation = true, TargetPreserved = true, WorkspacesCleaned = true });
}

var restricted = Directory.CreateDirectory(Path.Combine(root, "restricted"));
var originalAcl = restricted.GetAccessControl();
var descriptor = originalAcl.GetSecurityDescriptorBinaryForm();
using (var identity = WindowsIdentity.GetCurrent())
{
    originalAcl.AddAccessRule(new FileSystemAccessRule(identity.User!, FileSystemRights.CreateFiles, AccessControlType.Deny));
    restricted.SetAccessControl(originalAcl);
    try
    {
        try { using var unexpected = CutWorkspace.Create(Path.Combine(root, "denied.mp4"), restricted.FullName, new Log()); throw new InvalidOperationException("Write permission was ignored"); }
        catch (UnauthorizedAccessException) { proofs.Add(new { MissingWritePermissionRejected = true }); }
    }
    finally
    {
        var restore = new DirectorySecurity(); restore.SetSecurityDescriptorBinaryForm(descriptor, AccessControlSections.Access);
        restricted.SetAccessControl(restore);
    }
}
await CutOutputIdentityVerifier.ValidateAsync(source, sourceIdentity, true);
Console.WriteLine(JsonSerializer.Serialize(new { Passed = true, SourceUnchanged = true, Cases = proofs }, new JsonSerializerOptions { WriteIndented = true }));
static void Ensure(bool valid, string message) { if (!valid) throw new InvalidOperationException(message); }

sealed class Log(Action<CutProgressUpdate>? callback = null) : IProgress<CutProgressUpdate>
{
    private readonly object gate = new();
    private readonly List<string> lines = [];
    public readonly List<string> Workspaces = [];
    public void Report(CutProgressUpdate update)
    {
        lock (gate)
        {
            lines.Add(update.Message);
            const string prefix = "Tatsächlicher temporärer Arbeitsordner: ";
            if (update.Message.StartsWith(prefix)) Workspaces.Add(update.Message[prefix.Length..]);
        }
        callback?.Invoke(update);
    }
    public override string ToString() { lock (gate) return string.Join(Environment.NewLine, lines); }
}
