using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Engine.Model.Deserializers
{
    public class CsvModelDeserializer : IModelDeserializer
    {
        public Model Deserialize(string input)
        {
            using var reader = new StringReader(input);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<object>().ToArray();

            var cleaned = ObjectGraph.FixTypes(records);
            return new Model(cleaned);
        }

        public string Serialize(object o) => throw new NotImplementedException();
    }
}