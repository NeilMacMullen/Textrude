using System;

namespace Engine.Application;

/// <summary>
///     Allows us to treat the console (stdin/stdout) as a kind of limited file system
/// </summary>
/// <remarks>
///     Of course the console doesn't support paths in any meaningful way but it's useful to treat
///     '-' as a path indicating we should read from stdin or write to stdout.
/// </remarks>
public class ConsoleFileSystem : IFileSystemOperations
{
    public bool Exists(string path) => true;

    public string ReadAllText(string path) => Console.In.ReadToEnd();

    public DateTime GetLastWriteTimeUtc(string path) => DateTime.UtcNow;

    public void WriteAllText(string path, string content)
    {
        Console.WriteLine(content);
    }

    public bool CanHandle(string path) => path == "-";
}
