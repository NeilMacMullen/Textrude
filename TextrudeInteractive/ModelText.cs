using Engine.Application;

namespace TextrudeInteractive
{
    public record ModelText(string Text, ModelFormat Format);

    public static class ModelTexts
    {
        public static ModelText EmptyYaml = new ModelText(string.Empty, ModelFormat.Yaml);
    }

    /*
    {
    public ModelFormat Format = ModelFormat.Line;
    public string Text = string.Empty;

    public ModelText(string text, ModelFormat format)
    {
        Text = text;
        Format = format;
    }

    // for deserialization 
    public ModelText()
    {
    }

    public static ModelText EmptyYaml { get; } = new ModelText(string.Empty, ModelFormat.Yaml);
    }
*/
}