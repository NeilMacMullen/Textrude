using Engine.Model.Helpers;
using Newtonsoft.Json;

namespace Engine.Model.Deserializers
{
    public class JsonModelDeserializer : IModelDeserializer
    {
        public string Serialize(object o) => JsonConvert.SerializeObject(o);

        public Model Deserialize(string s)
        {
            var jobject = JsonGraph.GraphFromJsonString(s);
            return JsonGraph.Create(jobject);
        }
    }
}