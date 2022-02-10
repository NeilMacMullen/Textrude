using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban.Runtime;

namespace Engine.Model.Helpers;

/// <summary>
///     Class to walk a JObject tree and convert into a hierarchy of nested Dictionary string,object
/// </summary>
public class JsonGraph
{
    /// <summary>
    ///     Converts the supplied token into either raw value, or an array of values and adds these to the current node
    /// </summary>
    /// <remarks>
    ///     The topmost model can either be a ScriptObject or an array of ScriptObjects
    /// </remarks>
    public static void AddNamedObject(IDictionary<string, object> parent, string name, JToken token)
    {
        var obj = MakeTree(token);
        parent.Add(name, obj);
    }

    /// <summary>
    ///     Converts the supplied token into either raw value, or an array of values and adds these to the current node
    /// </summary>
    /// <remarks>
    ///     The topmost model can either be a ScriptObject or an array of ScriptObjects
    /// </remarks>
    public static Model Create(ModelFormat sourceFormat, JToken token) => new(sourceFormat, MakeTree(token));


    private static object MakeTree(JToken token)
    {
        var p = token;

        switch (p.Type)
        {
            case JTokenType.Array:
                var sos = p.Children().Select(MakeTree).ToArray();
                return sos;
            case JTokenType.Object:
            {
                var container = new ScriptObject();
                foreach (var prop in p.Value<JObject>().Properties())
                    AddNamedObject(container, prop.Name, prop.Value);

                return container;
            }

            case JTokenType.String:
                return p.Value<string>();

            case JTokenType.Integer:
                return p.Value<long>();

            case JTokenType.Float:
                return p.Value<double>();
            case JTokenType.Boolean:
                return p.Value<bool>();

            case JTokenType.Guid:
                return p.Value<Guid>();

            case JTokenType.Date:
                return p.Value<DateTime>();
            case JTokenType.Null:
                return null;
            /*
                            Unused types...
                            case JTokenType.None:
                            case JTokenType.Constructor:
                            case JTokenType.Property:
                            case JTokenType.Comment:

                            case JTokenType.Undefined:
                            case JTokenType.Raw:
                            case JTokenType.Bytes:
                            case JTokenType.Uri:
                            case JTokenType.TimeSpan:
            */
            default:
                throw new InvalidOperationException($"Don't know what to do with token of type {p.Type}");
        }
    }


    public static JToken GraphFromJsonString(string input) => JToken.Parse(input);

    public static JToken GraphFromObject(object o)
    {
        var json = JsonConvert.SerializeObject(o);
        return GraphFromJsonString(json);
    }


    public static object ToJsonSerialisableTree(object o, int depth = 0)
    {
        depth++;
        if (depth > 100)
            throw new InvalidOperationException("Attempt to serialise deeply nested or recursive structure");
        if (o is ScriptObject so)
        {
            return so
                .Where(kv => kv.Value != null)
                .ToDictionary(kv => kv.Key, kv => ToJsonSerialisableTree(kv.Value, depth));
        }

        if (o is IEnumerable<object> oArray)
        {
            return oArray.Select(p => ToJsonSerialisableTree(p, depth))
                .Where(v => v != null)
                .ToArray();
        }

        return o;
    }
}
