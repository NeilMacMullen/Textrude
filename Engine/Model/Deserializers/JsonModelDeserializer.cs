using Engine.Model.Helpers;
using Newtonsoft.Json;

namespace Engine.Model.Deserializers
{
    /// <summary>
    ///     Deserialises a JSON file
    /// </summary>
    public class JsonModelDeserializer : IModelDeserializer
    {
        public string Serialize(object o) => JsonConvert.SerializeObject(o, Formatting.Indented);

        public Model Deserialize(string s)
        {
            var jobject = JsonGraph.GraphFromJsonString(s);
            return JsonGraph.Create(ModelFormat.Json, jobject);
        }
    }
}
