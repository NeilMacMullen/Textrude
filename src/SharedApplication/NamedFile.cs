namespace SharedApplication
{
    /// <summary>
    ///     Simple record to hold both a path to a file and the associated name of the model/output
    /// </summary>
    public record NamedFile(string Name, string Path);
}
