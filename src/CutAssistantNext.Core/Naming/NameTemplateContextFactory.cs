using System.IO;

namespace CutAssistantNext.Core.Naming;

public static class NameTemplateContextFactory
{
    public static NameTemplateContext Create(
        string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            fileName);

        var originalFileName =
            Path.GetFileName(
                fileName);

        return OtrFileNameParser.TryParse(
            originalFileName,
            out var parsedContext)
            ? parsedContext
            : new NameTemplateContext(
                Name: Path.GetFileNameWithoutExtension(
                    originalFileName),
                OriginalName: originalFileName);
    }
}
