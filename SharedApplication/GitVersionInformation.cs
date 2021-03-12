namespace SharedApplication
{
#if ! HASGITVERSION
    /// <summary>
    ///     Provides version information
    /// </summary>
    /// <remarks>
    ///     This is normally supplied by the GitVersion package but some types of
    ///     builds don't have access to Git info
    /// </remarks>
    public static class GitVersionInformation
    {
        public const string UncommittedChanges = "0";
        public const string SemVer = "0.0.0";
        public const string CommitDate = "unknown";
        public const string ShortSha = "unknown";
    }
#endif
}
