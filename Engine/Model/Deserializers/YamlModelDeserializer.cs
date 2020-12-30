using System.IO;
using YamlDotNet.Serialization;

namespace Engine.Model.Deserializers
{
    public class YamlModelDeserializer : IModelDeserializer
    {
        public Model Deserialize(string input)
        {
            var r = new StringReader(input);
            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize(r);
            var d = ObjectGraph.FixTypes(yamlObject);
            return new Model(d);
        }

        public string Serialize(object o)
        {
            var serializer = new Serializer();
            return serializer.Serialize(o);
        }
    }
}