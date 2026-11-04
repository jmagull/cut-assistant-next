namespace CutAssistantNext.App.Tests;

// WPF resource/theme initialization is process-wide; these STA helpers must not compete across collections.
[CollectionDefinition("WpfUi", DisableParallelization = true)]
public sealed class WpfUiCollection { }
