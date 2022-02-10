using System;

namespace Engine.TemplateProcessing;

/// <summary>
///     A simple push/pop recogniser for PreprocessorDirectives
/// </summary>
/// <remarks>
///     We want to be able to turn off and on syntax extensions by pushing and
///     popping.  This class looks for lines like
///     {{# textrude push nosnarf}}
///     and maintains a count of pushes vs pops.  The stack is 'active' if the count
///     is zero.
/// </remarks>
public class ExtensionStack
{
    private readonly string _name;

    public ExtensionStack(string name) => _name = name;

    /// <summary>
    ///     True if the extension should process lines
    /// </summary>
    public bool IsActive => Depth == 0;

    /// <summary>
    ///     push/pop depth
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    ///     Tries to interpret line as a push/pop directive
    /// </summary>
    /// <remarks>
    ///     returns the IsActive status after the line has been examined
    /// </remarks>
    public bool CheckPushPop(string line)
    {
        var directive = new PreprocessorDirective(line);
        if (!directive.IsDirective)
            return IsActive;
        if (directive.Operand != _name)
            return IsActive;
        switch (directive.Command)
        {
            case "push":
                Push();
                break;
            case "pop":
                Pop();
                break;
        }

        return IsActive;
    }

    /// <summary>
    ///     Checks a line for push/pop then applies a transformation if active
    /// </summary>
    public string Process(string line, Func<string, string> transform)
        => CheckPushPop(line) ? transform(line) : line;

    /// <summary>
    ///     Checks the first line in a pair for push/pop then applies a transformation if active
    /// </summary>
    public LinePair Process(LinePair pair, Func<LinePair, LinePair> transform)
        => CheckPushPop(pair.First) ? transform(pair) : pair;

    public void Push() => Depth++;


    public void Pop() => Depth--;
}
