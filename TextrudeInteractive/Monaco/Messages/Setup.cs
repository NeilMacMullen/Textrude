

// ReSharper disable UnusedMember.Global

namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco to set up initial formatting
    /// </summary>
    public record Setup : MonacoMessages
    {
        public Setup(bool isReadOnly, string format)
        {
            IsReadOnly = isReadOnly;
            Language = format;
        }

        public bool IsReadOnly { get; }

        public string Language { get; }
    }
}
