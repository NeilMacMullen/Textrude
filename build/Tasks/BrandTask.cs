using Cake.Frosting;
using System;
using System.IO;

namespace Build.Tasks
{
    [TaskName("Brand")]
    [TaskDescription("Brands Textrude and TextrudeInteractive")]
    public sealed class BrandTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var commitDate = DateTime.Parse(context.Version.CommitDate);

            string versionInfo = $@"
using System;
using System.Reflection;

[assembly: AssemblyVersion(""{context.Version.AssemblySemVer}"")]
[assembly: AssemblyFileVersion(""{context.Version.AssemblySemFileVer}"")]
[assembly: AssemblyInformationalVersion(""{context.Version.InformationalVersion}"")]
[assembly: AssemblyCopyright(""Copyright (c) 2021 Textrude contributors"")]

internal static class VersionInfo {{
    public static readonly bool WasGenerated = true;
    public static readonly string SemanticVersion = ""{context.Version.SemVer}"";
    public static readonly DateTime CommitDate = new DateTime({commitDate.Year}, {commitDate.Month}, {commitDate.Day});
    public static readonly string CommitSha = ""{context.Version.Sha}"";
}}
            ";

            File.WriteAllText(context.VersionInfoFile, versionInfo);
        }
    }
}
