using System.Text.Json.Serialization;

namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change fontsize
    /// </summary>
    public record FontSize : MonacoMessages
    {
        public FontSize(double size) => Size = (int) size;

        [JsonPropertyName("size")] public int Size { get; }
    }
}
