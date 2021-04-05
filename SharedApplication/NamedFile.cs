using Engine.Model;

namespace SharedApplication
{
    /// <summary>
    ///     Simple record to hold both a path to a file and the associated name of the model/output
    /// </summary>
    public class NamedFile
    {
        public readonly ModelFormat Format;
        public readonly string Name;
        public readonly string Path;

        public NamedFile(string name, string path, ModelFormat format)
        {
            Format = format;
            Name = name;
            Path = path;
        }
    }
}
