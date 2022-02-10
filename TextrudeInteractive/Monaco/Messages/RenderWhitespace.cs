namespace TextrudeInteractive.Monaco.Messages;

/// <summary>
///     Sent TO Monaco tell it to show whitespace
/// </summary>
public record RenderWhitespace : MonacoMessages
{
    public enum MonacoWhitespaceType
    {
        Boundary,
        All,
        None,
    }

    public RenderWhitespace(MonacoWhitespaceType type) => WhitespaceType = type.ToString().ToLowerInvariant();

    public string WhitespaceType { get; }
}
