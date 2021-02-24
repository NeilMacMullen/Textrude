using System;
using System.Threading.Tasks;
using CommandLine;
using Engine.Application;

namespace Textrude
{
    public class CmdInfo
    {
        public static async Task Run(Options o, RunTimeEnvironment rte)
        {
            Console.WriteLine(
                @$"
 Version {VersionInfo.SemanticVersion} built {VersionInfo.CommitDate}   commit {VersionInfo.CommitSha} 

 Useful links:
  - Textrude homepage:      https://github.com/NeilMacMullen/Textrude
  - Scriban language docs:  https://github.com/scriban/scriban/blob/master/doc/language.md
  - Chat and questions:     https://gitter.im/Textrude/community
 ");

            if (VersionInfo.WasGenerated)
            {
                var latestVersion = await UpgradeManager.GetLatestVersion();
                if (latestVersion.Supersedes(VersionInfo.SemanticVersion))
                {
                    Console.WriteLine($"Upgrade to version {latestVersion.Version} available");
                    Console.WriteLine($"Please visit {UpgradeManager.ReleaseSite} for download");
                }
            }
        }

        [Verb("info", HelpText = "Provide detail information about version and further resources")]
        public class Options
        {
        }
    }
}
