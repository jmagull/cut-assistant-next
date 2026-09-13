using CutAssistantNext.Cutlists.IO;
using CutAssistantNext.Cutlists.Model;

namespace CutAssistantNext.App.Services;

internal static class CutlistServerUploadPayloadFactory
{
    internal static byte[] CreateBytes(
        CutlistDocument source)
    {
        ArgumentNullException.ThrowIfNull(
            source);

        var uploadDocument =
            CutlistServerUploadDocumentFactory.Create(
                source);

        return CutlistFileWriter.CreateBytes(
            uploadDocument);
    }
}