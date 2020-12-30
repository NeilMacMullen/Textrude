using System.Linq;
using Engine.Application;

namespace TextrudeInteractive
{
    public class GenInput
    {
        public readonly string[] Definitions;
        public readonly ModelFormat Format;
        public readonly string ModelText;
        public readonly string Template;

        public GenInput(string template, string modelText, ModelFormat format, string definitionsText)
        {
            Template = template;
            ModelText = modelText;
            Format = format;
            Definitions = definitionsText
                .Split('\r', '\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToArray();
        }

        public static GenInput EmptyYaml { get; } =
            new GenInput(string.Empty, string.Empty, ModelFormat.Yaml, string.Empty);
    }
}