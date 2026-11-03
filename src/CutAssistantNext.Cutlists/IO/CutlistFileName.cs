namespace CutAssistantNext.Cutlists.IO;

public static class CutlistFileName
{
    public static string FromOriginal(string originalFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        var fileName = Path.GetFileName(originalFileName);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Der Originaldateiname für die Cutlist fehlt.", nameof(originalFileName));

        fileName += ".cutlist";
        if (fileName.Length > 255 || fileName.Any(Path.GetInvalidFileNameChars().Contains))
            throw new ArgumentException(
                "Aus dem Originaldateinamen kann kein gültiger Cutlist-Dateiname erzeugt werden. " +
                "Der Name einschließlich .cutlist darf höchstens 255 Zeichen enthalten.", nameof(originalFileName));
        return fileName;
    }
}
