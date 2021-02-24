namespace Engine.Application
{
    /// <summary>
    ///     Supported Model file formats
    /// </summary>
    public enum ModelFormat
    {
        Json,
        Csv,
        Yaml,

        /// <summary>
        ///     File will be read as an array of lines
        /// </summary>
        Line
    }
}