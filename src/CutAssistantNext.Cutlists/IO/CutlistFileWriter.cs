using System.Text;
using CutAssistantNext.Cutlists.Model;
using CutAssistantNext.Cutlists.Serialization;

namespace CutAssistantNext.Cutlists.IO;

public static class CutlistFileWriter
{
    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    public static void Write(
        string filePath,
        CutlistDocument document)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            filePath);

        ArgumentNullException.ThrowIfNull(document);

        var fullPath =
            Path.GetFullPath(filePath);

        var directoryPath =
            Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(
                directoryPath);
        }

        var content =
            CutlistSerializer.Serialize(document);

        content = NormalizeToCrLf(content);

        File.WriteAllText(
            fullPath,
            content,
            Utf8WithoutBom);
    }

    private static string NormalizeToCrLf(
        string value)
    {
        return value
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", "\r\n");
    }
}
