using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedGetOnlyAutoProperty
namespace TextrudeInteractive.Monaco.Messages
{
    /// <summary>
    ///     Sent FROM Monaco when the user has typed into the edit pane
    /// </summary>
    public record UpdatedText : MonacoMessages
    {
        [JsonPropertyName("text")] public string Text { get; }
    }
}
