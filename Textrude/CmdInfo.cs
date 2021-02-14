﻿using System;
using System.Threading.Tasks;
using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdInfo
    {
        public static async Task Run(Options o, RunTimeEnvironment rte)
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
 - Chat and questions:     https://gitter.im/Textrude/community
");

            var latestVersion = await UpgradeManager.GetLatestVersion();
            if (latestVersion.Supersedes(GitVersionInformation.SemVer))
            {
                Console.WriteLine($"Upgrade to version {latestVersion.Version} available");
                Console.WriteLine($"Please visit {UpgradeManager.ReleaseSite} for download");
            }

#endif
        }

        [Verb("info", HelpText = "Provide detail information about version and further resources")]
        public class Options
        {
        }
    }
}
