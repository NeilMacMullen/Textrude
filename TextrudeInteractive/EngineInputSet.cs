using System;
using System.Linq;

namespace TextrudeInteractive
{
    public record EngineInputSet
    {
        public EngineInputSet(string template, ModelText[] models, string definitionsText,
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

        public string[] Definitions { get; init; } = Array.Empty<string>();
        public string[] IncludePaths { get; init; } = Array.Empty<string>();

        public ModelText[] Models { get; init; } = Array.Empty<ModelText>();

        public string Template { get; init; } = string.Empty;

        public static EngineInputSet EmptyYaml { get; } =
            new EngineInputSet(string.Empty, Array.Empty<ModelText>(), string.Empty, string.Empty);
    }
}