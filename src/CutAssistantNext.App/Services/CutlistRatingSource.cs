using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CutAssistantNext.App.Settings;

namespace CutAssistantNext.App.Services;

// Only the download/import path creates this provenance; a local filename is never sufficient.
internal sealed class CutlistRatingSource
{
    internal string PersonalServerUrl { get; }
    internal Uri Endpoint { get; }
    internal string Fred { get; }
    internal long CutlistId { get; }
    internal string Author { get; }
    internal string Name { get; }
    internal string Key { get; }

    private CutlistRatingSource(string url, Uri endpoint, string fred, long id, string author, string name)
    {
        PersonalServerUrl = url;
        Endpoint = endpoint;
        Fred = fred;
        CutlistId = id;
        Author = author;
        Name = name;
        // Neither the credential nor a reversible personal URL is persisted in the history.
        Key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            endpoint.GetLeftPart(UriPartial.Authority) + "\n" + fred + "\n" + id.ToString(CultureInfo.InvariantCulture))));
    }

    internal static CutlistRatingSource? Create(string url, string id, string author, string name)
    {
        if (!CutlistServerSettingsValidator.TryValidatePersonalServerUrl(url, out _) ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.UserInfo.Length != 0 ||
            !uri.IsDefaultPort || uri.AbsolutePath.Length != 66 ||
            !long.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var numericId) || numericId <= 0)
            return null;
        return new(url, new Uri(uri, "/api/set-rating"), uri.AbsolutePath[1..^1], numericId, author.Trim(), name);
    }

    public override string ToString() => $"Server-Cutlist {CutlistId}";
}

internal sealed class UsedCutlistRatingContext(CutlistRatingSource source, string mediaPath)
{
    internal CutlistRatingSource Source { get; } = source;
    internal string MediaPath { get; } = mediaPath;
    public override string ToString() => Source.ToString();
}
