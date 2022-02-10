namespace Engine.Model;

public interface IModelDeserializer
{
    public Model Deserialize(string s);
    public string Serialize(object o);
}
