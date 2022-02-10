// ReSharper disable UnusedMember.Global

namespace TextrudeInteractive.Monaco.Messages;

/// <summary>
///     Sent TO Monaco when we want the edit pane to take new text content
/// </summary>
public record UpdateText : MonacoMessages
{
    public UpdateText(string text) => Text = text;

    public string Text { get; }
}
