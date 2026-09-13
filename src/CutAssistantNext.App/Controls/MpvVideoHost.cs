using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace CutAssistantNext.App.Controls;

public sealed class MpvVideoHost : HwndHost
{
    private const uint WindowStyleChild = 0x40000000;
    private const uint WindowStyleVisible = 0x10000000;
    private const uint WindowStyleClipChildren = 0x02000000;
    private const uint WindowStyleClipSiblings = 0x04000000;

    private const uint StaticStyleBlackRectangle = 0x00000004;

    private nint _videoWindowHandle;
    private (int Left, int Top, int Right, int Bottom)? _lastClip;

    public MpvVideoHost()
    {
        Loaded += (_, _) => LayoutUpdated += UpdateViewportClip;
        Unloaded += (_, _) => LayoutUpdated -= UpdateViewportClip;
    }

    protected override void OnWindowPositionChanged(Rect rcBoundingBox)
    {
        base.OnWindowPositionChanged(rcBoundingBox);
        UpdateViewportClip(this, EventArgs.Empty);
    }

    private void UpdateViewportClip(object? sender, EventArgs e)
    {
        if (_videoWindowHandle == 0 || PresentationSource.FromVisual(this) is null)
        {
            return;
        }

        var visible = new Rect(RenderSize);
        for (DependencyObject? parent = VisualTreeHelper.GetParent(this);
             parent is not null;
             parent = VisualTreeHelper.GetParent(parent))
        {
            if (parent is ScrollContentPresenter viewport)
            {
                visible.Intersect(viewport.TransformToDescendant(this)
                    .TransformBounds(new Rect(viewport.RenderSize)));
            }
        }

        var dpi = VisualTreeHelper.GetDpi(this);
        var clip = visible.IsEmpty
            ? (Left: 0, Top: 0, Right: 0, Bottom: 0)
            : (Left: (int)Math.Ceiling(visible.Left * dpi.DpiScaleX),
               Top: (int)Math.Ceiling(visible.Top * dpi.DpiScaleY),
               Right: (int)Math.Floor(visible.Right * dpi.DpiScaleX),
               Bottom: (int)Math.Floor(visible.Bottom * dpi.DpiScaleY));

        if (_lastClip == clip)
        {
            return;
        }

        var region = CreateRectRgn(clip.Left, clip.Top, clip.Right, clip.Bottom);
        if (region == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        // Windows owns the region after a successful SetWindowRgn call.
        // SetWindowRgn sends position messages synchronously; cache before calling it.
        var previousClip = _lastClip;
        _lastClip = clip;
        if (SetWindowRgn(_videoWindowHandle, region, true) == 0)
        {
            var error = Marshal.GetLastWin32Error();
            _lastClip = previousClip;
            _ = DeleteObject(region);
            throw new Win32Exception(error);
        }
    }

    public event EventHandler? VideoWindowHandleCreated;

    public event EventHandler? VideoWindowHandleDestroyed;

    public nint VideoWindowHandle =>
        _videoWindowHandle;

    public bool IsVideoWindowReady =>
        _videoWindowHandle != 0;

    protected override HandleRef BuildWindowCore(
        HandleRef hwndParent)
    {
        var windowStyle =
            WindowStyleChild |
            WindowStyleVisible |
            WindowStyleClipChildren |
            WindowStyleClipSiblings |
            StaticStyleBlackRectangle;

        var windowHandle = CreateWindowExW(
            0,
            "STATIC",
            string.Empty,
            windowStyle,
            0,
            0,
            0,
            0,
            hwndParent.Handle,
            0,
            0,
            0);

        if (windowHandle == 0)
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "Das native Videofenster konnte nicht erstellt werden.");
        }

        _videoWindowHandle = windowHandle;

        VideoWindowHandleCreated?.Invoke(
            this,
            EventArgs.Empty);

        return new HandleRef(
            this,
            windowHandle);
    }

    protected override void DestroyWindowCore(
        HandleRef hwnd)
    {
        var windowHandle = hwnd.Handle;

        if (windowHandle != 0)
        {
            _ = DestroyWindow(windowHandle);
        }

        _videoWindowHandle = 0;
        _lastClip = null;

        VideoWindowHandleDestroyed?.Invoke(
            this,
            EventArgs.Empty);
    }

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern nint CreateRectRgn(int left, int top, int right, int bottom);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowRgn(nint window, nint region,
        [MarshalAs(UnmanagedType.Bool)] bool redraw);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint handle);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode,
        SetLastError = true)]
    private static extern nint CreateWindowExW(
        uint extendedStyle,
        string className,
        string windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parentWindowHandle,
        nint menuHandle,
        nint instanceHandle,
        nint parameter);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(
        nint windowHandle);
}
