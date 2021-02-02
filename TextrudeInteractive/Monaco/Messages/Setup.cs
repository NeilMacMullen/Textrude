using System.Text.Json.Serialization;

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

        [JsonPropertyName("isReadOnly")] public bool IsReadOnly { get; }

        [JsonPropertyName("language")] public string Language { get; }
    }
}
