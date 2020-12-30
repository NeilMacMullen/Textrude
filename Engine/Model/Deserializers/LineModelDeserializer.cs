using System;

namespace Engine.Model.Deserializers
{
    public class LineModelDeserializer : IModelDeserializer
    {
        public Model Deserialize(string input)
        {
            var lines = input.Split(Environment.NewLine);
            return new Model(lines);
        }

        public string Serialize(object o)
        {
            throw new NotImplementedException();
        }
    }
}