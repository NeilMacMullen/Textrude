using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Scriban.Runtime;

namespace Engine.Application
{
    public static class ExtensionCache
    {
        public enum KnownAssemblies
        {
            Debug,
            Humanizr
        }

        private static readonly Dictionary<KnownAssemblies, ScriptObject> _cachedResults =
            new();

        public static ScriptObject GetHumanizrMethods()
        {
            return GetOrCreate(KnownAssemblies.Humanizr,
                () =>
                {
                    //force a load of the DLL otherwise we won't see the types
                    "force load".Humanize();
                    return AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Single(a => a.FullName.Contains("Humanizer"))
                        .GetTypes()
                        .Where(t => t.Name.EndsWith("Extensions"))
                        .ToArray();
                });
        }


        private static ScriptObject GetOrCreate(KnownAssemblies name, Func<IEnumerable<Type>> typeFetcher)
        {
            if (_cachedResults.TryGetValue(name, out var scriptObject))
                return scriptObject;
            scriptObject = new ScriptObject();
            foreach (var extensionClass in typeFetcher())
                scriptObject.Import(extensionClass);
            _cachedResults[name] = scriptObject;

            return scriptObject;
        }

        public static ScriptObject GetDebugMethods()
        {
            return GetOrCreate(KnownAssemblies.Debug, () => new[] {typeof(DebugMethods)});
        }

        public static ScriptObject MakeScriptObject(IEnumerable<Type> types)
        {
            var scriptObject1 = new ScriptObject();
            foreach (var extensionClass in types)
            {
                scriptObject1.Import(extensionClass);
            }

            return scriptObject1;
        }
    }
}