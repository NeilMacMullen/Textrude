namespace Engine.Model;

/// <summary>
///     Supported Model file formats
/// </summary>
/// <remarks>
///     Integer values are used for serialization
/// </remarks>
public enum ModelFormat
{
    Json = 0,
    Csv = 1,
    Yaml = 2,

    /// <summary>
    ///     File will be read as an array of lines
    /// </summary>
    Line = 3,

    //not actually applicable - used as a placeholder for uncategorised input
    Unknown = -1,
}
