namespace CutAssistantNext.Media.Playback.Interop;

internal interface ILibMpvClient : IAsyncDisposable
{
    IAsyncEnumerable<LibMpvEvent> ReadEventsAsync(
        CancellationToken cancellationToken = default);

    Task InitializeAsync(
        nint videoWindowHandle,
        CancellationToken cancellationToken = default);

    Task CommandAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);

    Task ObservePropertyAsync(
        string propertyName,
        CancellationToken cancellationToken = default);

    Task SetBooleanPropertyAsync(
        string propertyName,
        bool value,
        CancellationToken cancellationToken = default);

    Task SetDoublePropertyAsync(
        string propertyName,
        double value,
        CancellationToken cancellationToken = default);
}

internal enum LibMpvEventKind
{
    FileLoaded,
    PlaybackRestarted,
    EndFile,
    PropertyChanged,
    Error
}

internal sealed record LibMpvEvent(
    LibMpvEventKind Kind,
    string? PropertyName = null,
    object? Value = null,
    string? ErrorMessage = null);