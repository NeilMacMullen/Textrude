using System;

namespace Engine.Application
{
    /// <summary>
    ///     Represents a Textrude pre-processor directive
    /// </summary>
    /// <remarks>
    ///     It's possible to issue instructions to the pre-processor using Scriban comments of the form
    ///     {{# textrude command operand }}
    ///     This class tries to parse a line into a directive
    /// </remarks>
    public class PreprocessorDirective
    {
        /// <summary>
        ///     Indicates the start of a directive
        /// </summary>
        private const string Flag = "textrude";

        /// <summary>
        ///     The command (lower-cased)
        /// </summary>
        public readonly string Command = string.Empty;

        /// <summary>
        ///     True if the directive is well-formed
        /// </summary>
        public readonly bool IsDirective;

        /// <summary>
        ///     The operand (lower-cased)
        /// </summary>
        public readonly string Operand = string.Empty;

        /// <summary>
        ///     Parse and construct a directive from a string
        /// </summary>
        /// <remarks>
        ///     IsDirective will be false if the line can't be parsed
        /// </remarks>
        public PreprocessorDirective(string line)
        {
            //ignore leading whitespace
            line = line.Trim();

            //must start with Scriban code block
            if (!line.StartsWith("{{"))
                return;

            //skip past comment and white-space munching operators
            line = line.Substring(2);
            while (line.Length != 0 && " #~-".Contains(line[0]))
                line = line.Substring(1);

            //turn closing brace into spaces
            line = line.Replace("}}", " ");
            //now get the tokens
            var tokens = line
                .ToLowerInvariant()
                .Split(" ",
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

            if (tokens.Length < 3)
                return;

            //first must be textrude
            if (tokens[0] != Flag)
                return;

            Command = tokens[1];
            Operand = tokens[2];
            IsDirective = true;
        }
    }
}
