namespace TextrudeInteractive.SettingsManagement;

/// <summary>
///     Defines a snippet as used by Monaco
/// </summary>
/// <remarks>
///     See
///     https://microsoft.github.io/monaco-editor/playground.html#extending-language-services-completion-provider-example
/// </remarks>
public record Snippet
{
    public string Label { get; init; } = string.Empty;
    public string Documentation { get; init; } = string.Empty;
    public string InsertText { get; init; } = string.Empty;
}
