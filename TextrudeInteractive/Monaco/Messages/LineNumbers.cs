using System.Text.Json.Serialization;

namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent TO Monaco tell it to change fontsize
    /// </summary>
    public record LineNumbers : MonacoMessages
    {
        public LineNumbers(bool onOff) => Enabled = MonacoOptions.OnOff(onOff);

        [JsonPropertyName("enabled")] public string Enabled { get; }
    }

    /// <summary>
    ///     Sent TO Monaco tell it to change fontsize
    /// </summary>
    public record WordWrap : MonacoMessages
    {
        public WordWrap(bool onOff) => Enabled = MonacoOptions.OnOff(onOff);

        [JsonPropertyName("enabled")] public string Enabled { get; }
    }
}
