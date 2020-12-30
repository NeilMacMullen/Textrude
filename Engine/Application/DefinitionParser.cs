using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Application
{
    public static class DefinitionParser
    {
        private static string[] SplitToken(string str)
        {
            var separator = str.IndexOf('=');

            if (separator <= 0 || separator >= str.Length)
                throw new ArgumentException($"'{str}' is not a valid definition");

            var id = str.Substring(0, separator).Trim();

            if (id.Length <= 0)
                throw new ArgumentException($"'{str}' is not a valid definition");

            return new[] {id, str.Substring(separator + 1)};
        }

        public static Dictionary<string, string> CreateDefinitions(IEnumerable<string> rawDefinitions)
        {
            var definitions =
                rawDefinitions
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
}