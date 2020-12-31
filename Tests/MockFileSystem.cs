using System;
using System.Collections.Generic;
using System.IO;
using Engine.Application;

namespace Tests
{
    /// <summary>
    ///     Simple mock of filesystem to let us test application
    /// </summary>
    internal class MockFileSystem : IFileSystemOperations
    {
        private readonly Dictionary<string, DatedContent> _files = new();

        public bool Exists(string path) => _files.ContainsKey(path);


        public string ReadAllText(string path) =>
            Exists(path)
                ? _files[path].Content
                : throw new IOException("file not present");

        public DateTime GetLastWriteTimeUtc(string path) =>
            Exists(path)
                ? _files[path].LastWriteTime
                : throw new IOException("file not present");


        public void WriteAllText(string path, string content)
        {
            var dc = new DatedContent(content, DateTime.UtcNow);
            _files[path] = dc;
        }

        public string ApplicationFolder() => "exeFolder";

        private record DatedContent
        {
            public readonly string Content;
            public readonly DateTime LastWriteTime;

            public DatedContent(string content, DateTime utcNow)
            {
                Content = content;
                LastWriteTime = utcNow;
            }
        }
    }
}