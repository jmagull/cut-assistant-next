using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Threading;
using CutAssistantNext.App.Services;

namespace CutAssistantNext.App.Controls;

[ComVisible(true), Guid("00000122-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVideoFileDropTarget
{
    [PreserveSig] int DragEnter([MarshalAs(UnmanagedType.Interface)] IDataObject data, uint keys, long point, ref uint effect);
    [PreserveSig] int DragOver(uint keys, long point, ref uint effect);
    [PreserveSig] int DragLeave();
    [PreserveSig] int Drop([MarshalAs(UnmanagedType.Interface)] IDataObject data, uint keys, long point, ref uint effect);
}

/// <summary>OLE bridge for the native video surface; WPF's surrounding border shares its validation.</summary>
[ComVisible(true), ClassInterface(ClassInterfaceType.None)]
internal sealed class VideoFileDropTarget : IVideoFileDropTarget, IDisposable
{
    private readonly Func<bool> _canOpen;
    private readonly Action<string> _open;
    private readonly Action<bool> _highlight;
    private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
    private string[] _files = [];
    private string? _path;
    private int _generation;
    private nint _window;
    private bool _disposed;

    internal VideoFileDropTarget(Func<bool> canOpen, Action<string> open, Action<bool> highlight)
    {
        _canOpen = canOpen;
        _open = open;
        _highlight = highlight;
    }

    internal void Register(nint window)
    {
        if (_window != 0) return;
        Marshal.ThrowExceptionForHR(RegisterDragDrop(window, this));
        _window = window;
    }

    internal bool CanDrop => !_disposed && _path is not null && _canOpen();

    internal void SetFiles(string[] files)
    {
        if (_disposed || _files.SequenceEqual(files, StringComparer.Ordinal)) return;
        Leave();
        _files = files.ToArray();
        try
        {
            if (_canOpen() && MediaOpenRequest.IsDropCandidate(files))
                _ = ValidateAsync(files[0], _generation);
        }
        catch (ArgumentException) { }
    }

    private async Task ValidateAsync(string path, int generation)
    {
        string? resolved = null;
        try { resolved = await MediaOpenRequest.ResolveAsync(path, Environment.CurrentDirectory, requireVideoExtension: true).ConfigureAwait(false); }
        catch (Exception) { /* Rejected drag data must never escape an OLE callback. */ }
        if (_dispatcher.HasShutdownStarted || _dispatcher.HasShutdownFinished) return;
        try
        {
            await _dispatcher.InvokeAsync(() =>
            {
                if (_disposed || generation != _generation) return;
                _path = resolved;
                _highlight(CanDrop);
            });
        }
        catch (TaskCanceledException) { /* The owning window is shutting down. */ }
    }

    internal void Leave()
    {
        ++_generation;
        _files = [];
        _path = null;
        _highlight(false);
    }

    internal bool AcceptDrop()
    {
        var path = CanDrop ? _path : null;
        Leave();
        if (path is null) return false;
        // Return from OLE before a confirmation dialog or asynchronous load begins.
        _dispatcher.BeginInvoke(() => { if (!_disposed && _canOpen()) _open(path); });
        return true;
    }

    public int DragEnter(IDataObject data, uint keys, long point, ref uint effect)
    {
        SetFiles(ReadFiles(data));
        effect = CanDrop ? effect & 1u : 0;
        return 0;
    }

    public int DragOver(uint keys, long point, ref uint effect)
    {
        _highlight(CanDrop);
        effect = CanDrop ? effect & 1u : 0;
        return 0;
    }

    public int DragLeave() { Leave(); return 0; }

    public int Drop(IDataObject data, uint keys, long point, ref uint effect)
    {
        var sameFiles = _files.SequenceEqual(ReadFiles(data), StringComparer.Ordinal);
        var allowed = (effect & 1u) != 0 && sameFiles && AcceptDrop();
        if (!allowed) Leave();
        effect = allowed ? 1u : 0;
        return 0;
    }

    private static string[] ReadFiles(IDataObject data)
    {
        var format = new FORMATETC
        {
            cfFormat = 15,
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = TYMED.TYMED_HGLOBAL
        };
        try
        {
            if (data.QueryGetData(ref format) != 0) return [];
            data.GetData(ref format, out var medium);
            try
            {
                if (medium.tymed != TYMED.TYMED_HGLOBAL || medium.unionmember == 0 ||
                    DragQueryFileW(medium.unionmember, uint.MaxValue, null, 0) != 1) return [];
                var length = DragQueryFileW(medium.unionmember, 0, null, 0);
                if (length == 0 || length > 32767) return [];
                var path = new StringBuilder((int)length + 1);
                _ = DragQueryFileW(medium.unionmember, 0, path, length + 1);
                return [path.ToString()];
            }
            finally { ReleaseStgMedium(ref medium); }
        }
        catch (COMException) { return []; }
    }

    public void Dispose()
    {
        if (_disposed) return;
        Leave();
        _disposed = true;
        if (_window != 0) { _ = RevokeDragDrop(_window); _window = 0; }
    }

    [DllImport("ole32.dll")] private static extern int RegisterDragDrop(nint window, IVideoFileDropTarget target);
    [DllImport("ole32.dll")] private static extern int RevokeDragDrop(nint window);
    [DllImport("ole32.dll")] private static extern void ReleaseStgMedium(ref STGMEDIUM medium);
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern uint DragQueryFileW(nint drop, uint index, StringBuilder? file, uint size);
}
