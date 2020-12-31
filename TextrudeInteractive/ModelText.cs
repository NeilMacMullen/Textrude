using Engine.Application;

namespace TextrudeInteractive
{
    public class ModelText
    {
        public ModelText(string text, ModelFormat format)
        {
            Text = text;
            Format = format;
        }

        public static ModelText EmptyYaml { get; } = new(string.Empty, ModelFormat.Yaml);


        public ModelFormat Format { get; init; } = ModelFormat.Line;
        public string Text { get; init; } = string.Empty;
    }
}