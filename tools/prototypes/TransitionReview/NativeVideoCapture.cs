using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CutAssistantNext.Media.Playback;

namespace TransitionReview;

// Test-only client API bridge: read the frame already held by the actual CAN libmpv instance.
internal static class NativeVideoCapture
{
    private static object Context(MpvMediaPlayerService player)
    {
        var client = typeof(MpvMediaPlayerService).GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(player)!;
        return client.GetType().GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(client)!;
    }

    public static void EnableLog(MpvMediaPlayerService player) => Context(player).GetType().GetProperty("LogEnabled")!.SetValue(Context(player), true);

    public static unsafe string Capture(MpvMediaPlayerService player, string path)
    {
        var context = Context(player);
        var handle = (nint)Pointer.Unbox(context.GetType().GetProperty("Ctx", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(context)!);
        var strings = new[] { "screenshot-raw", "video", "bgra" }.Select(Marshal.StringToCoTaskMemUTF8).ToArray();
        var arguments = Marshal.AllocHGlobal((strings.Length + 1) * nint.Size);
        var result = new Node();
        var success = false;
        try
        {
            for (var i = 0; i < strings.Length; i++) Marshal.WriteIntPtr(arguments, i * nint.Size, strings[i]);
            Marshal.WriteIntPtr(arguments, strings.Length * nint.Size, 0);
            var code = mpv_command_ret(handle, arguments, out result);
            if (code < 0)
                throw new IOException($"libmpv screenshot-raw: {code}; {context.GetType().GetProperty("Log")!.GetValue(context)}");
            success = true;
            if (result.Format != 8) throw new InvalidDataException("Keine Bilddaten von libmpv.");
            var map = Marshal.PtrToStructure<NodeList>(result.Pointer);
            var values = new Dictionary<string, Node>(StringComparer.Ordinal);
            for (var i = 0; i < map.Count; i++)
                values.Add(Marshal.PtrToStringUTF8(Marshal.ReadIntPtr(map.Keys, i * nint.Size))!,
                    Marshal.PtrToStructure<Node>(map.Values + i * Marshal.SizeOf<Node>()));
            var width = checked((int)values["w"].Integer);
            var height = checked((int)values["h"].Integer);
            var stride = checked((int)values["stride"].Integer);
            var format = Marshal.PtrToStringUTF8(values["format"].Pointer);
            var data = Marshal.PtrToStructure<ByteArray>(values["data"].Pointer);
            if (format != "bgra" || width <= 0 || height <= 0 || stride < checked(width * 4) ||
                data.Size < checked((nuint)stride * (nuint)height))
                throw new NotSupportedException("Unerwartetes libmpv-Bildformat.");
            var rowSize = checked(width * 4);
            var pixels = new byte[checked(rowSize * height)];
            for (var y = 0; y < height; y++) Marshal.Copy(data.Data + y * stride, pixels, y * rowSize, rowSize);
            var image = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, rowSize);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write)) encoder.Save(stream);
            return Convert.ToHexString(SHA256.HashData(pixels));
        }
        finally
        {
            if (success) mpv_free_node_contents(ref result);
            Marshal.FreeHGlobal(arguments);
            foreach (var value in strings) Marshal.FreeCoTaskMem(value);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct Node
    {
        [FieldOffset(0)] public nint Pointer;
        [FieldOffset(0)] public long Integer;
        [FieldOffset(8)] public int Format;
    }
    [StructLayout(LayoutKind.Sequential)] private struct NodeList { public int Count; public nint Values; public nint Keys; }
    [StructLayout(LayoutKind.Sequential)] private struct ByteArray { public nint Data; public nuint Size; }
    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int mpv_command_ret(nint handle, nint arguments, out Node result);
    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void mpv_free_node_contents(ref Node result);
}
