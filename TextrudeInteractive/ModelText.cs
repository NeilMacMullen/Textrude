using Engine.Application;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Holds text that can be parsed to become a Model
    /// </summary>
    public record ModelText
    {
        public ModelText(string text, ModelFormat format)
        {
            Text = text;
            Format = format;
        }

        /// <summary>
        ///     Useful default object
        /// </summary>
        public static ModelText EmptyYaml { get; } = new(string.Empty, ModelFormat.Yaml);


        public ModelFormat Format { get; init; } = ModelFormat.Line;
        public string Text { get; init; } = string.Empty;
    }
}