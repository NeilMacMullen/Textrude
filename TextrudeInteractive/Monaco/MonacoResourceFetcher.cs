using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using TextrudeInteractive.Properties;

namespace TextrudeInteractive.Monaco;

/// <summary>
///     Implements fetching of resources from within the embedded Monaco zipped image
/// </summary>
/// <remarks>
///     This is broken out here in case we decide to add caching and in order to simplify the
///     binding code.
/// </remarks>
public class MonacoResourceFetcher
{
    public static readonly MonacoResourceFetcher Instance = new();
    private ImmutableArray<string> _supportedLanguages = ImmutableArray<string>.Empty;

    private MonacoResourceFetcher()
    {
    }

    private byte[] GetMonacoResource() => Resources.monaco_editor_0_22_3;

    public ImmutableArray<string> GetSupportedFormats()
    {
        if (!_supportedLanguages.Any())
        {
            var monacoLangRegex = new Regex(@"vs/(basic-languages|language)/(?<name>.+)/$");
            using var zipStream = new MemoryStream(GetMonacoResource());
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
            _supportedLanguages = zip.Entries
                .Select(e => monacoLangRegex.Match(e.FullName))
                .Where(m => m.Success)
                .Select(m => m.Groups["name"].Value)
                .Concat(new[] { "text", "scriban","kusto" })
                .OrderBy(l => l)
                .Distinct()
                .ToImmutableArray();
        }

        return _supportedLanguages;
    }

    public MemoryStream FetchPath(string path)
    {
        Debug.WriteLine($"Fetching {path}");
        if (path.Contains("kusto"))
        {
            Debug.WriteLine("HERE!!!");
            return new MemoryStream();
        }

        if (path.EndsWith("scriban.js"))
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(Resources.scriban));
        }

        using var zipStream = new MemoryStream(GetMonacoResource());
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var file = zip.GetEntry(path);
        var response = new MemoryStream(); // cache into local stream so is not disposed
        file.Open().CopyTo(response);
        response.Position = 0;
        return response;
    }

    public MemoryStream Monaco() => new(Encoding.UTF8.GetBytes(Resources.monaco));


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
