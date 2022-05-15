namespace TextrudeInteractive.Monaco.Messages;

public record CompletionNode(string name, string description, string text, CompletionType kind)
{
}
