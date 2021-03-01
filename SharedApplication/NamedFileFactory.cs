using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Engine.Application;

namespace SharedApplication
{
    /// <summary>
    ///     Creates NamedFiles from raw text input
    /// </summary>
    public static class NamedFileFactory
    {
        public static NamedFile SplitAssignment(string raw, string fallbackName)
        {
            var match = Regex.Match(raw, @"((\w+)!)?((\w+)=)?(.*)");
            var format = ModelFormat.Unknown;
            if (match.Groups[2].Success
                && Enum.TryParse(typeof(ModelFormat), match.Groups[2].Value, true, out var f))
                format = (ModelFormat) f;
            var modelName = match.Groups[4].Success
                ? match.Groups[4].Value
                : fallbackName;
            var path = match.Groups[5].Value;
            return new NamedFile(modelName, path, format);
        }

        public static ImmutableArray<NamedFile> ToNamedFiles(IEnumerable<string> raw,
            Func<int, string> defaultNamer
        )
        {
            return raw
                .Select(
                    (r, i) => SplitAssignment(r, defaultNamer(i))
                )
                .ToImmutableArray();
        }

        public static ImmutableArray<string> Squash(IEnumerable<NamedFile> files)
        {
            string formatString(NamedFile f) => f.Format == ModelFormat.Unknown
                ? string.Empty
                : $"{f.Format}!";

            return files.Select(f => $"{formatString(f)}{f.Name}={f.Path}").ToImmutableArray();
        }
    }
}
