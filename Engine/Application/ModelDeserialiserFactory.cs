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

        public static IModelDeserializer Fetch(ModelFormat type)
        {
            return KnownDeserializers[type];
        }

        public static string Serialise(object o, ModelFormat type)
        {
            return Fetch(type).Serialize(o);
        }
    }
}