using System.IO;
using Engine.Model.Helpers;
using YamlDotNet.Serialization;

namespace Engine.Model.Deserializers;

/// <summary>
///     Deserializer for YAML
/// </summary>
public class YamlModelDeserializer : IModelDeserializer
{
    public Model Deserialize(string input)
    {
        var r = new StringReader(input);
        var deserializer = new Deserializer();
        var yamlObject = deserializer.Deserialize(r);
        var d = ObjectGraph.FixTypes(yamlObject);
        return new Model(ModelFormat.Yaml, d);
    }

    public string Serialize(object o)
    {
        var serializer = new Serializer();
        return serializer.Serialize(o);
    }
}
