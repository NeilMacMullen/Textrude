using System;
using System.Linq;

namespace TextrudeInteractive
{
    public record GenInput
    {
        public string[] Definitions = Array.Empty<string>();
        public string[] IncludePaths = Array.Empty<string>();

        public ModelText[] Models = Array.Empty<ModelText>();

        public string Template = string.Empty;

        //for deserialization
        public GenInput()
        {
        }

        public GenInput(string template, ModelText[] models, string definitionsText,
            string includes)
        {
            Template = template;
            Models = models;

            Definitions = definitionsText
                .Split('\r', '\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToArray();

            IncludePaths = includes.Split(Environment.NewLine,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        public static GenInput EmptyYaml { get; } =
            new GenInput(string.Empty, Array.Empty<ModelText>(), string.Empty, string.Empty);
    }
}