using System;
using CommandLine;

namespace Textrude
{
    public class CmdTest
    {
        public static void Run(Options o)
        {
            Console.WriteLine(GitVersionInformation.FullSemVer);
            Console.WriteLine("Textrude homepage:  https://github.com/NeilMacMullen/Textrude");
            Console.WriteLine("Scriban language docs:  https://github.com/scriban/scriban/blob/master/doc/language.md");
        }

        [Verb("test")]
        public class Options
        {
        }
    }
}