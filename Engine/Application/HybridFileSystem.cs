using System;
using System.Linq;

namespace Engine.Application
{
    /// <summary>
    ///     Allows multiple FileSystem types to be stacked
    /// </summary>
    /// <remarks>
    ///     Allows us to fetch things from a real file, a URL,
    ///     or the console
    ///     NOTE that sources are considered in order.
    /// </remarks>
    public class HybridFileSystem : IFileSystemOperations
    {
        private readonly IFileSystemOperations[] _handlers;

        public HybridFileSystem(params IFileSystemOperations[] handlers) => _handlers = handlers;

        public bool Exists(string path) => Route(path).Exists(path);

        public string ReadAllText(string path) => Route(path).ReadAllText(path);

        public DateTime GetLastWriteTimeUtc(string path) => Route(path).GetLastWriteTimeUtc(path);

        public void WriteAllText(string path, string content) => Route(path).WriteAllText(path, content);
        public bool CanHandle(string path) => true;
        public ModelFormat DefaultFormat(string path) => Route(path).DefaultFormat(path);

        private IFileSystemOperations Route(string path)
        {
            return _handlers.First(h => h.CanHandle(path));
        }
    }
}
