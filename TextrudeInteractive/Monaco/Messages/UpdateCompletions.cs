namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change syntax highlighting
    /// </summary>
    public record UpdateCompletions : MonacoMessages
    {
        public UpdateCompletions(Completions c) => Completions = c;

        public Completions Completions { get; }
    }
}
