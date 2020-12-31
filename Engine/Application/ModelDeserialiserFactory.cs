using System.Collections.Generic;
using Engine.Model;
using Engine.Model.Deserializers;

namespace Engine.Application
{
    public static class ModelDeserializerFactory
    {
        private static readonly Dictionary<ModelFormat, IModelDeserializer>
            KnownDeserializers = new()
            {
                [ModelFormat.Json] = new JsonModelDeserializer(),
                [ModelFormat.Csv] = new CsvModelDeserializer(),
                [ModelFormat.Yaml] = new YamlModelDeserializer(),
                [ModelFormat.Line] = new LineModelDeserializer()
            };

        public static IModelDeserializer Fetch(ModelFormat type) => KnownDeserializers[type];

        public static string Serialise(object o, ModelFormat type) => Fetch(type).Serialize(o);

        public static ModelFormat FormatFromExtension(string extension)
        {
            foreach (var s in KnownDeserializers.Keys)
            {
                if (extension.ToUpperInvariant().EndsWith(s.ToString().ToUpperInvariant()))
                    return s;
            }

            //if in doubt just treat the input as lines of text
            return ModelFormat.Line;
        }
    }
}