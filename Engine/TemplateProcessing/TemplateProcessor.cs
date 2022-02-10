using System;
using System.Linq;

namespace Engine.TemplateProcessing;

/// <summary>
///     Used to preprocess a template string
/// </summary>
/// <remarks>
///     This is the place we apply all our syntax extensions.  The main public API
///     is the ApplyAllTransforms factory method
/// </remarks>
public class TemplateProcessor
{
    private string[] _lines;

    public TemplateProcessor(string template) => _lines = template.ToLines().ToArray();

    /// <summary>
    ///     Gets the final transformed string
    /// </summary>
    public string Template => string.Join(Environment.NewLine, _lines);


    /// <summary>
    ///     Applies all transformations in the processor to the supplied string
    /// </summary>
    public static string ApplyAllTransforms(string template)
    {
        var processor = new TemplateProcessor(template);
        processor.FunctionSnarf();
        processor.TerseLambda();
        processor.HoistPipes();
        return processor.Template;
    }

    /// <summary>
    ///     Provides the extended pipe syntax
    /// </summary>
    /// <remarks>
    ///     Allows us to 'hoist' pipes from the beginning of the line below
    ///     so that Scriban can see them
    /// </remarks>
    public void HoistPipes()
    {
        const string pipe = "|>";
        var stack = new ExtensionStack("nopipehoist");

        static LinePair Hoist(LinePair p)
        {
            var trimmedSecond = p.Second.TrimStart();
            return trimmedSecond.StartsWith(pipe)
                ? new LinePair(p.First + $" {pipe}", trimmedSecond.Substring(pipe.Length))
                : p;
        }

        _lines = _lines.ProcessLinePairs(p => stack.Process(p, Hoist)).ToArray();
    }

    /// <summary>
    ///     Provides whitespace snarfing at the beginning of functions
    /// </summary>
    public void FunctionSnarf()
    {
        var stack = new ExtensionStack("nofuncsnarf");
        static string StartFunc(string line) => line.Replace("{{func", "{{-func");
        _lines = _lines
            .Select(line => stack.Process(line, StartFunc))
            .ToArray();
    }

    /// <summary>
    ///     Provides terse lambda syntax
    /// </summary>
    public void TerseLambda()
    {
        var stack = new ExtensionStack("noterselambda");
        static string StartFunc(string line) => line.Replace("@{", "@(do;ret (");
        static string EndFunc(string line) => line.Replace("}@", ");end)");

        _lines = _lines
            .Select(line => stack.Process(line, StartFunc))
            .Select(line => stack.Process(line, EndFunc))
            .ToArray();
    }
}
