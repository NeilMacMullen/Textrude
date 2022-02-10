namespace TextrudeInteractive.Monaco.Messages;

/// <summary>
///     Sent TO Monaco tell it to change fontsize
/// </summary>
public record FontSize : MonacoMessages
{
    public FontSize(double size) => Size = (int)size;

    public int Size { get; }
}
