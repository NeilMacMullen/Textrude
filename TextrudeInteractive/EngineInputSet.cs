using System;
using System.Linq;

namespace TextrudeInteractive;

/// <summary>
///     Holds all the input necessary to run a template rendering pass
/// </summary>
/// <remarks> </remarks>
public record EngineInputSet
{
    public static EngineInputSet EmptyYaml =
        new(string.Empty, string.Empty, Array.Empty<ModelText>(), string.Empty, string.Empty);

    public EngineInputSet(string template, string templatePath, ModelText[] models, string definitionsText,
        string includes)
    {
        Template = template;
        TemplatePath = templatePath;
        Models = models;
        Definitions = Lines(definitionsText);
        IncludePaths = Lines(includes);
    }

    //Required for deserialisation
    public EngineInputSet()
    {
    }

    /// <summary>
    /// </summary>
    public string[] Definitions { get; init; } = Array.Empty<string>();

    public string[] IncludePaths { get; init; } = Array.Empty<string>();

    public ModelText[] Models { get; init; } = Array.Empty<ModelText>();

    public string Template { get; init; } = string.Empty;
    public string TemplatePath { get; init; } = string.Empty;

    private string[] Lines(string str)
    {
        return str
            .Split(new[] { '\r', '\n' }, StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries)
            .Where(p => !p.StartsWith("#"))
            .ToArray();
    }
}
