using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Engine.Application
{
    /// <summary>
    ///     Provides a way of checking whether the current version is out of date
    /// </summary>
    /// <remarks>
    ///     We look for new versions on the github page
    /// </remarks>
    public static class UpgradeManager
    {
        public const string ReleaseSite = "https://github.com/NeilMacMullen/Textrude";

        private
            static readonly Uri ChangeList =
                new("https://raw.githubusercontent.com/NeilMacMullen/Textrude/master/doc/changelist.json");

        public static async Task<VersionInfo> GetLatestVersion()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var raw = await client.GetStringAsync(ChangeList);
                    var infos = JsonConvert.DeserializeObject<VersionInfo[]>(raw);
                    return infos.OrderByDescending(i => i.Date).First();
                }
            }
            catch
            {
                return VersionInfo.Default;
            }
        }

        public static (int major, int minor, int patch) DecomposeVersion(string v)
        {
            try
            {
                var elements = v.Split(".")
                    .Select(int.Parse)
                    .ToArray();
                return (elements[0], elements[1], elements[2]);
            }
            catch
            {
                return (0, 0, 0);
            }
        }

        public static int CompareVersions(string a, string b)
        {
            var av = DecomposeVersion(a);
            var bv = DecomposeVersion(b);
            var d = av.major - bv.major;
            if (d != 0) return d;

            d = av.minor - bv.minor;
            if (d != 0) return d;
            d = av.patch - bv.patch;
            if (d != 0) return d;

            return 0;
        }

        public record VersionInfo
        {
            /// <summary>
            ///     The default value with sensible fields
            /// </summary>
            public static readonly VersionInfo Default = new();

            public string Version { get; init; } = "0.0.0";


            public DateTime Date { get; init; }
            public string Summary { get; init; } = string.Empty;

            public bool Supersedes(string currentVersion)
                => CompareVersions(Version, currentVersion) > 0;
        }
    }
}