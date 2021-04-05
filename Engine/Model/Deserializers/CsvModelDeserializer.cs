using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Engine.Model.Deserializers
{
    /// <summary>
    ///     Deseriazes a CSV file
    /// </summary>
    public class CsvModelDeserializer : IModelDeserializer
    {
        public Model Deserialize(string input)
        {
            using var reader = new StringReader(input);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<object>().ToArray();

            var cleaned = ObjectGraph.FixTypes(records);
            return new Model(ModelFormat.Csv, cleaned);
        }

        public string Serialize(object o)
        {
            if (o is not IEnumerable<object> r) return string.Empty;
            if (!r.Any()) return string.Empty;
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            if (r.First() is not Dictionary<string, object> header)
                return string.Empty;

            foreach (var kv in header.OrderBy(i => i.Key))
                csv.WriteField(kv.Key);
            csv.NextRecord();

            foreach (var f in r)
            {
                if (f is not Dictionary<string, object> rr)
                    continue;
                foreach (var kv in rr.OrderBy(i => i.Key))
                    csv.WriteField(kv.Value);
                csv.NextRecord();
            }

            return writer.ToString();
        }
    }
}
