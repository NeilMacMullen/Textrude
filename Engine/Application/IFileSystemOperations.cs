using System;

namespace Engine.Application
{
    /// <summary>
    ///     Provides an abstraction of the filesystem to facilitate testing
    /// </summary>
    public interface IFileSystemOperations
    {
        public bool Exists(string path);
        public string ReadAllText(string path);
        public DateTime GetLastWriteTimeUtc(string path);
        public void WriteAllText(string path, string content);
        string ApplicationFolder();
    }
}