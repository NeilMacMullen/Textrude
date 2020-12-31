using System;
using System.Linq;
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

        /* for deserialization */
        public ModelText()
        {
        }

        public ModelFormat Format { get; set; } = ModelFormat.Line;
        public string Text { get; set; } = string.Empty;
        public static ModelText EmptyYaml { get; } = new ModelText(string.Empty, ModelFormat.Yaml);
    }

    public class GenInput
    {
        public readonly ModelText[] Models = Array.Empty<ModelText>();
        public readonly string Template = string.Empty;

        //for deserialization
        public GenInput()
        {
        }

        public GenInput(string template, ModelText[] models, string definitionsText)
        {
            Template = template;
            Models = models;

            Definitions = definitionsText
                .Split('\r', '\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToArray();
        }

        public string[] Definitions { get; set; }

        public static GenInput EmptyYaml { get; } =
            new GenInput(string.Empty, Array.Empty<ModelText>(), string.Empty);
    }
}