// ReSharper disable UnusedMember.Global

namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change syntax highlighting
    /// </summary>
    public record UpdateLanguage : MonacoMessages
    {
        public UpdateLanguage(string language) => Language = language.ToLowerInvariant();

        public string Language { get; }
    }
}
