using Engine.Application;
using Engine.Model;

namespace Engine.Extensions;

/// <summary>
///     Provides some helpful functions for templates
/// </summary>
public static class DebugMethods
{
    /// <summary>
    ///     Use the YAML serializer to dump objects
    /// </summary>
    public static string Dump(object o)
    {
        var serializer = ModelDeserializerFactory.Fetch(ModelFormat.Yaml);
        var text = serializer.Serialize(o);
        return text;
    }
}
