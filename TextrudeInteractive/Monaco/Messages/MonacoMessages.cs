using System.Text.Json;

namespace TextrudeInteractive.Monaco.Messages;

/// <summary>
///     Abstract base class for all messages sent between Monaco editor and WebView
/// </summary>
public abstract record MonacoMessages
{
    protected MonacoMessages() => Type = GetType().Name;

    public string Type { get; }

    public string ToJson() => JsonSerializer.Serialize(this, GetType());

    public static T FromJson<T>(string json)
        => JsonSerializer.Deserialize<T>(json);
}
