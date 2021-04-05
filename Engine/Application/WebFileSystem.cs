using System;
using System.Net.Http;

namespace Engine.Application
{
    /// <summary>
    ///     Allows use to fetch files from the web
    /// </summary>
    /// <remarks>
    ///     It's debatable whether this should really be a feature of Textrude but doing things with JSON
    ///     files is quite convenient.
    /// </remarks>
    public class WebFileSystem : IFileSystemOperations
    {
        public bool Exists(string path) => true;

        public string ReadAllText(string path)
        {
            using var client = new HttpClient();
            //Some servers get upset if we don't have a user-agent
            client.DefaultRequestHeaders.Add("User-Agent", "Textrude");
            var t = client.GetStringAsync(path);
            t.Wait();
            return t.Result;
        }

        public DateTime GetLastWriteTimeUtc(string path) => DateTime.UtcNow;

        public void WriteAllText(string path, string content)
        {
            //do nothing
        }

        public bool CanHandle(string path) => path.StartsWith("http://") || path.StartsWith("https://");
    }
}
