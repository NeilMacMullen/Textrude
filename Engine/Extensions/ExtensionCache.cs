﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Extensions.TimeRange;
using Engine.TemplateProcessing;
using Humanizer;
using Scriban.Runtime;

namespace Engine.Extensions;

public static class ExtensionCache
{
    public enum KnownAssemblies
    {
        Debug,
        Humanizr,
        Misc,
        Textrude,
        Group,
        TimeComparison
    }

    private static readonly Dictionary<KnownAssemblies, ScriptObject> CachedResults =
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
                    .Single(a => a.FullName.EmptyWhenNull().Contains("Humanizer"))
                    .GetTypes()
                    .Where(t => t.Name.EndsWith("Extensions"))
                    .ToArray();
            });
    }


    private static ScriptObject GetOrCreate(KnownAssemblies name, Func<IEnumerable<Type>> typeFetcher)
    {
        if (CachedResults.TryGetValue(name, out var scriptObject))
            return scriptObject;
        scriptObject = new ScriptObject();
        foreach (var extensionClass in typeFetcher())
            scriptObject.Import(extensionClass);
        CachedResults[name] = scriptObject;

        return scriptObject;
    }

    public static ScriptObject GetDebugMethods() =>
        GetOrCreate(KnownAssemblies.Debug, () => new[] { typeof(DebugMethods) });


    public static ScriptObject GetMiscMethods() =>
        GetOrCreate(KnownAssemblies.Misc, () => new[] { typeof(MiscMethods) });

    public static ScriptObject GetTextrudeMethods() =>
        GetOrCreate(KnownAssemblies.Textrude, () => new[] { typeof(TextrudeMethods) });


    public static ScriptObject GetGroupingMethods() =>
        GetOrCreate(KnownAssemblies.Group, () => new[] { typeof(Group) });

    public static ScriptObject GetTimeComparisonMethods() =>
        GetOrCreate(KnownAssemblies.TimeComparison, () => new[] { typeof(TimeRangeMethods) });
}
