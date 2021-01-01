using System;
using CommandLine;

namespace Textrude
{
    public class CmdInfo
    {
        public static void Run(Options o)
        {
#if HASGITVERSION
            var isDirty = GitVersionInformation.UncommittedChanges == "0"
                ? ""
                : " (dirty build)";
            Console.WriteLine(
                @$"
Version {GitVersionInformation.SemVer} built {GitVersionInformation.CommitDate}   commit {GitVersionInformation.ShortSha} {isDirty} 

Useful links:
 - Textrude homepage:      https://github.com/NeilMacMullen/Textrude
 - Scriban language docs:  https://github.com/scriban/scriban/blob/master/doc/language.md
");
#endif
        }

        [Verb("info")]
        public class Options
        {
        }
    }
}