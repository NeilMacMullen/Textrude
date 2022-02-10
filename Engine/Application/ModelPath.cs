using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Engine.Application;

/// <summary>
///     Representation of the path to a model node
/// </summary>
/// <remarks>
///     This class is used to assist with code-completion
/// </remarks>
public class ModelPath
{
    public enum PathType
    {
        Method,
        Property,
        Unknown,
        Keyword
    }

    public const string Separator = ".";

    /// <summary>
    ///     The empty path - can be used as a root
    /// </summary>
    public static readonly ModelPath Empty = new(ImmutableArray<string>.Empty);

    private readonly ImmutableArray<string> _tokens;

    private ModelPath(ImmutableArray<string> tokens) => _tokens = tokens;

    public bool IsEmpty => Length == 0;

    public string RootToken => IsEmpty ? string.Empty : _tokens[0];

    /// <summary>
    ///     True if this path has descendants
    /// </summary>
    public bool IsParent => _tokens.Length > 1;

    /// <summary>
    ///     Length of path (# elements)
    /// </summary>
    public int Length => _tokens.Length;

    public PathType ModelType { get; private set; } = PathType.Unknown;

    public static ModelPath FromTokens(IEnumerable<string> tokens)
    {
        var tokenArray = tokens.ToImmutableArray();
        return tokenArray.Any() ? new ModelPath(tokenArray) : Empty;
    }

    public ModelPath Child() => FromTokens(_tokens.Skip(1));

    /// <summary>
    ///     Returns a new path that has the specified descendant
    /// </summary>
    public ModelPath WithChild(string child) => FromTokens(_tokens.Append(child));

    public ModelPath WithType(PathType type)
    {
        var m = FromTokens(_tokens);
        m.ModelType = type;
        return m;
    }


    /// <summary>
    ///     Returns a "dotted" string representation
    /// </summary>
    public string Render() => string.Join(Separator, _tokens);

    public static ModelPath FromString(string path) =>
        FromTokens((path.Split(Separator)));

    public string Terminal() => IsEmpty ? string.Empty : _tokens.Last();
}
