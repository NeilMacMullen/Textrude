using System;
using System.Collections.Generic;
using System.Linq;
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
                [ModelFormat.Line] = new LineModelDeserializer(),
                [ModelFormat.Unknown] = new AutoDeserializer(),
            };

        public static IModelDeserializer Fetch(ModelFormat type) => KnownDeserializers[type];

        public static string Serialise(object o, ModelFormat type) => Fetch(type).Serialize(o);

        /// <summary>
        ///     Figure out the likely format of a model from the extension of the file that contains it
        /// </summary>
        public static ModelFormat FormatFromPathOrExtension(string pathOrExtension)
        {
            //TODO_- make the deserializers provide their own list of supported extensions
            foreach (var s in KnownDeserializers.Keys)
            {
                var ext = pathOrExtension.ToUpperInvariant();
                var fmt = s.ToString().ToUpperInvariant();
                if (ext == fmt)
                    return s;
                if (ext.EndsWith("." + fmt))
                    return s;
            }

            return ModelFormat.Unknown;
        }
    }


    public class AutoDeserializer : IModelDeserializer
    {
        public Model.Model Deserialize(string s) => ModelDeserializerFactory.Fetch(GuessFormat(s)).Deserialize(s);

        public string Serialize(object o) => throw new NotImplementedException();


        public static ModelFormat GuessFormatFromPath(string path) =>
            ModelDeserializerFactory.FormatFromPathOrExtension(path);

        public static ModelFormat GuessFormat(string s)
        {
            //try to guess the format by looking at the content

            var t = s.Substring(0, Math.Min(s.Length, 1000)).Trim();

            //if it starts with a brace, it's probably json
            if (t.StartsWith("{") || t.StartsWith("["))
                return ModelFormat.Json;

            var lines = t.ToLines();
            //if all the lines contain a comma, it's probably a CSV
            if (lines.Count() > 1 && lines.All(l => l.Contains(",")))
                return ModelFormat.Csv;

            //if first line contains a colon or starts with '-' it could be yaml
            if (lines.Any())
            {
                var firstLine = lines.First();
                if (firstLine.Trim().StartsWith("-") || firstLine.Contains(":"))
                    return ModelFormat.Yaml;
            }

            //ok - give up and try lines
            return ModelFormat.Line;
        }
    }
}
