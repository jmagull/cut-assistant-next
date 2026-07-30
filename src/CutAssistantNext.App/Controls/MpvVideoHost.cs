using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace CutAssistantNext.App.Controls;

public sealed class MpvVideoHost : HwndHost
{
    private const uint WindowStyleChild = 0x40000000;
    private const uint WindowStyleVisible = 0x10000000;
    private const uint WindowStyleClipChildren = 0x02000000;
    private const uint WindowStyleClipSiblings = 0x04000000;

    private const uint StaticStyleBlackRectangle = 0x00000004;

    private nint _videoWindowHandle;

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

        VideoWindowHandleDestroyed?.Invoke(
            this,
            EventArgs.Empty);
    }

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