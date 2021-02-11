namespace SharedApplication
{
    /// <summary>
    ///     Provide a sensible name for a model or output based on the provided index
    /// </summary>
    public static class NameProvider
    {
        private static string Get(string prefix, int n)
            => $"{prefix}{(n == 0 ? "" : n)}";

        public static string IndexedModel(int n)
            => Get("model", n);

        public static string IndexedOutput(int n)
            => Get("output", n);
    }
}
