using System;
using System.Collections.Generic;

namespace Engine.Application
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Transforms a set of lines by operating on each successive pair
        /// </summary>
        /// <remarks>
        ///     Similar to Pairwise but this is expected to transform the lines in-situ,  For example
        ///     when transforming [1,2,3] we might want to start by turning (1,2) into ("one","two")
        ///     then continue with transforming ("two",3)
        /// </remarks>
        public static IEnumerable<string> ProcessLinePairs(this IEnumerable<string> lines,
            Func<LinePair, LinePair> transform)
        {
            string previous = null;
            foreach (var line in lines)
            {
                if (previous != null)
                {
                    var (first, second) = transform(new LinePair(previous, line));
                    previous = second;
                    yield return first;
                }
                else
                    previous = line;
            }

            if (previous != null)
                yield return previous;
        }

        /// <summary>
        ///     Splits a string into lines
        /// </summary>
        public static IEnumerable<string> ToLines(this string str) => str.Split(Environment.NewLine);
    }
}
