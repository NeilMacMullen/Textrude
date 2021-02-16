using System.IO;
using Engine.Application;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Allows conversion between absolute and relative paths
    /// </summary>
    public class PathManipulator
    {
        /// <summary>
        ///     The "root" from which relative paths are calculated
        /// </summary>
        public readonly string Root;

        public PathManipulator(string root) => Root = root;

        /// <summary>
        ///     Returns a path relative to the root
        /// </summary>
        /// <remarks>
        ///     start by converting to absolute in case we are already relative!
        /// </remarks>
        public string ToRelative(string path) => path.Length == 0 ? path : Path.GetRelativePath(Root, ToAbsolute(path));


        public static bool IsAbsolute(string path)
            => path.Length != 0 && Path.IsPathRooted(path);

        /// <summary>
        ///     Returns an absolute (rooted) path
        /// </summary>
        /// <remarks>
        ///     If a relative path is passed in we assume it is relative to the root
        /// </remarks>
        public string ToAbsolute(string path) => path.Length == 0 ? path : Path.GetFullPath(path, Root);

        /// <summary>
        ///     Static factory method for a manipulator rooted in the current exe folder since that is quite common
        /// </summary>
        public static PathManipulator FromExe()
        {
            //TODO - this is a rather roundabout way of the getting the current application folder!
            var exePath =
                Path.Combine(new RunTimeEnvironment(new FileSystemOperations()).ApplicationFolder());
            return new PathManipulator(exePath);
        }
    }
}
