using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using TextrudeInteractive.Properties;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Implements fetching of resources from within the embedded Monaco zipped image
    /// </summary>
    /// <remarks>
    ///     This is broken out here in case we decide to add caching and in order to simplify the
    ///     binding code.
    /// </remarks>
    public class MonacoResourceFetcher
    {
        private ImmutableArray<string> _supportedLanguages = ImmutableArray<string>.Empty;

        public ImmutableArray<string> GetSupportedFormats()
        {
            if (!_supportedLanguages.Any())
            {
                var monacoLangRegex = new Regex(@"vs/(basic-languages|language)/(?<name>.+)/$");
                using var zipStream = new MemoryStream(Resources.monaco_editor_0_21_2);
                using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
                _supportedLanguages = zip.Entries
                    .Select(e => monacoLangRegex.Match(e.FullName))
                    .Where(m => m.Success)
                    .Select(m => m.Groups["name"].Value)
                    .Concat(new[] {"text"})
                    .OrderBy(l => l)
                    .Distinct()
                    .ToImmutableArray();
            }

            return _supportedLanguages;
        }

        public MemoryStream FetchPath(string path)
        {
            using var zipStream = new MemoryStream(Resources.monaco_editor_0_21_2);
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
            var file = zip.GetEntry(path);
            var response = new MemoryStream(); // cache into local stream so is not disposed
            file.Open().CopyTo(response);
            response.Position = 0;
            return response;
        }

        public MemoryStream Monaco() => new MemoryStream(Encoding.UTF8.GetBytes(Resources.monaco));


        public static bool IsWebView2RuntimeAvailable()
        {
            try
            {
                var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                return !string.IsNullOrWhiteSpace(version);
            }
            catch
            {
                return false;
            }
        }
    }
}
