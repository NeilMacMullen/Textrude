using Engine.Application;

namespace TextrudeInteractive
{
    public class ModelText
    {
        public static ModelText EmptyYaml = new ModelText(string.Empty, ModelFormat.Yaml);

        public ModelText(string text, ModelFormat format)
        {
            Text = text;
            Format = format;
        }


        public ModelFormat Format { get; init; } = ModelFormat.Line;
        public string Text { get; init; } = string.Empty;
    }
}