using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharedApplication
{
    /// <summary>
    ///     Creates NamedFiles from raw text input
    /// </summary>
    public static class NamedFileFactory
    {
        public static NamedFile SplitAssignment(string raw, string fallbackName)
        {
            var match = Regex.Match(raw, @"(\w+)=(.+)");
            return match.Success
                ? new NamedFile(match.Groups[1].Value, match.Groups[2].Value)
                : new NamedFile(fallbackName, raw);
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
            return files.Select(f => $"{f.Name}={f.Path}").ToImmutableArray();
        }
    }
}
