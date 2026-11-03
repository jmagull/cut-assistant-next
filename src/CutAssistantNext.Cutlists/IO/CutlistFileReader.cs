using System.Text;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.IO;

public static class CutlistFileReader
{
    private static readonly UTF8Encoding Utf8WithoutBom =
        new(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true);

    private static readonly Encoding Windows1252 =
        CreateWindows1252Encoding();

    public static CutlistDocument Read(
        string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            filePath);

        var fullFilePath =
            Path.GetFullPath(
                filePath);

        if (!File.Exists(fullFilePath))
        {
            throw new FileNotFoundException(
                "Die Cutlist-Datei wurde nicht gefunden.",
                fullFilePath);
        }

        string content;

        try
        {
            content =
                File.ReadAllText(
                    fullFilePath,
                    Utf8WithoutBom);
        }
        catch (DecoderFallbackException)
        {
            content =
                File.ReadAllText(
                    fullFilePath,
                    Windows1252);
        }

        return CutlistParser.Parse(
            content);
    }

    private static Encoding CreateWindows1252Encoding()
    {
        Encoding.RegisterProvider(
            CodePagesEncodingProvider.Instance);

        return Encoding.GetEncoding(
            1252);
    }

    /// <summary>Decodes an already read snapshot, using the same BOM and legacy encoding rules as Read.</summary>
    public static CutlistDocument ReadBytes(byte[] bytes, int? maximumCuts = null)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        string content;
        try { content = Decode(bytes, Utf8WithoutBom); }
        catch (DecoderFallbackException) { content = Decode(bytes, Windows1252); }
        return CutlistParser.Parse(content, maximumCuts);
    }

    private static string Decode(byte[] bytes, Encoding encoding)
    {
        using var stream = new MemoryStream(bytes, writable: false);
        using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }
}
