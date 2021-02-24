using System.Collections.Generic;
using Engine.Model;
using Engine.Model.Deserializers;

namespace Engine.Application
{
    /// <summary>
    ///     Factory to help us "do the right thing" depending on the format of the model
    /// </summary>
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

        /// <summary>
        ///     Figure out the likely format of a model from the extension of the file that contains it
        /// </summary>
        public static ModelFormat FormatFromExtension(string extension)
        {
            //TODO_- make the deserializers provide their own list of supported extensions
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