using System.Collections.Generic;
using System.Linq;
using Engine.Model;
using Engine.Model.Helpers;
using Scriban.Runtime;

namespace Engine.Application
{
    /// <summary>
    ///     A set of methods that are meant to assist with setting up Textrude infrastructure
    /// </summary>
    public static class TextrudeMethods
    {
        /// <summary>
        ///     Global Scriban variable used to hold dynamic output table
        /// </summary>
        public const string DynamicOutputName = "dynamic_output";

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

        /// <summary>
        ///     Allows an application to register "dynamic" output
        /// </summary>
        /// <remarks>
        ///     The first parameter should be the Scriban "this" object. The second is the
        ///     name of the output file.  The last is the content
        /// </remarks>
        public static void AddOutput(object thisObject, string outputName, string content)
        {
            if (thisObject is not ScriptObject top) return;
            var outputs = new Dictionary<string, string>();
            if (!top.TryGetValue(DynamicOutputName, out var dynOutput))
            {
                top.SetValue(DynamicOutputName, outputs, false);
            }
            else
            {
                outputs = dynOutput as Dictionary<string, string>;
            }

            outputs[outputName] = content;
        }

        /// <summary>
        ///     Allows scriban code to preprocess other code
        /// </summary>
        /// <remarks>
        ///     Useful for auto-doc generation
        /// </remarks>
        public static string PreProcess(string text) =>
            text == null ? string.Empty : TemplateProcessor.ApplyAllTransforms(text);


        public static string Serialize(object o, ModelFormat format)
        {
            if (o == null)
                return string.Empty;
            return ModelDeserializerFactory.Serialise(JsonGraph.ToJsonSerialisableTree(o), format);
        }

        public static string ToJson(object o) => Serialize(o, ModelFormat.Json);
        public static string ToCsv(object o) => Serialize(o, ModelFormat.Csv);
        public static string ToYaml(object o) => Serialize(o, ModelFormat.Yaml);
        public static string ToLine(object o) => Serialize(o, ModelFormat.Line);
    }
}
