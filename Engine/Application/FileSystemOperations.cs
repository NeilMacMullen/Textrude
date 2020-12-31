using System;
using System.IO;
using System.Reflection;

namespace Engine.Application
{
    public class FileSystemOperations : IFileSystemOperations
    {
        public bool Exists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public string ApplicationFolder() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}