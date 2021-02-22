using System.Linq;
using Scriban.Runtime;

namespace Engine.Application
{
    /// <summary>
    ///     A set of methods that are meant to assist with setting up Textrude infrastructure
    /// </summary>
    public static class TextrudeMethods
    {
        /// <summary>
        ///     Allows functions and variables created in root object to be moved to library
        /// </summary>
        /// <remarks>
        ///     The first parameter should be the Scriban "this" object. The second is the
        ///     name of the library we want to create.  The convention is that funcs/objects
        ///     declared as __libname_myobj are moved to libname.myobj
        /// </remarks>
        public static void CreateLibrary(object thisObject, string libraryName)
        {
            if (thisObject is not ScriptObject top) return;
            var prefix = $"__{libraryName}_";

            var lib = new ScriptObject();

            var libraryMembers = top
                .Where(kv => kv.Key.StartsWith(prefix))
                .ToArray();
            foreach (var (key, value) in libraryMembers)
            {
                var newName = key.Substring(prefix.Length);
                lib.SetValue(newName, value, true);
            }

            top.SetValue(libraryName, lib, true);
        }
    }
}
