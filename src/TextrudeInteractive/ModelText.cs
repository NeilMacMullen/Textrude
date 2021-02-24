using Engine.Application;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Holds text that can be parsed to become a Model
    /// </summary>
    public record ModelText
    {
        public ModelText(string text, ModelFormat format, string name, string path)
        {
            Text = text ?? string.Empty;
            Format = format;
            Name = name ?? string.Empty;
            Path = path ?? string.Empty;
        }

        /// <summary>
        ///     Useful default object
        /// </summary>
        public static ModelText EmptyYaml { get; } = new(string.Empty, ModelFormat.Yaml, string.Empty, string.Empty);


        public ModelFormat Format { get; init; } = ModelFormat.Line;
        public string Text { get; init; } = string.Empty;
        public string Path { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }
}
