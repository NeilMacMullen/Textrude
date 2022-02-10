using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Application;

/// <summary>
///     Provides parsing for user-supplied definitions
/// </summary>
/// <remarks>
///     The application allows the user to import definitions into the template context.
///     This is done by supplying assignments in the format
///     name=value
///     The parser trims names but NOT values.  (This policy may change)
///     Empty assignments are allowed; i.e.
///     'def=' is a valid assignment and will result in def having the empty string as a value.
/// </remarks>
public static class DefinitionParser
{
    /// <summary>
    ///     Parse a single assignment
    /// </summary>
    private static string[] SplitToken(string str)
    {
        var separatorIndex = str.IndexOf('=');

        if (separatorIndex <= 0)
            throw new ArgumentException($"'{str}' is not a valid definition");

        var id = str.Substring(0, separatorIndex).Trim();

        if (id.Length <= 0)
            throw new ArgumentException($"'{str}' is not a valid definition");

        return new[] { id, str.Substring(separatorIndex + 1) };
    }

    /// <summary>
    ///     Parse a set of assignments and return them as a dictionary
    /// </summary>
    /// <remarks>
    ///     Models can accept dictionaries directly so that's our preferred output format
    /// </remarks>
    public static Dictionary<string, string> CreateDefinitions(IEnumerable<string> definitionAssignments)
    {
        var definitions =
            definitionAssignments
                .Where(d => d.Length > 0)
                .Select(SplitToken)
                .ToArray();
        var duplicates = definitions.Select(a => a[0]).GroupBy(n => n)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();


        if (duplicates.Any())
            throw new ArgumentException(
                $"The following definitions were duplicated: {string.Join(",", duplicates)}");

        return definitions.ToDictionary(a => a[0], a => a[1]);
    }
}
