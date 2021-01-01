namespace Engine.Application
{
    /// <summary>
    ///     Provides some helpful functions for templates
    /// </summary>
    public static class HelperFunctions
    {
        /// <summary>
        ///     Use the YAML serializer to dump objects
        /// </summary>
        public static string Dump(object o)
        {
            var serialiser = ModelDeserializerFactory.Fetch(ModelFormat.Yaml);
            var text = serialiser.Serialize(o);
            return text;
        }
    }
}