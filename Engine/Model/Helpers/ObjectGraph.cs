using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Scriban.Runtime;

namespace Engine.Model.Deserializers
{
    public static class ObjectGraph
    {
        /// <summary>
        ///     Hack
        /// </summary>
        /// <remarks>
        ///     Quick hack to turn strings that look like numbers into
        ///     number types.  The better approach would be to walk the YamlDocument
        ///     and use the presence or lack of quotes around the token to
        ///     determine if a string was intended
        /// </remarks>
        public static object FixTypes(object tree)
        {
            switch (tree)
            {
                case string s:
                {
                    if (long.TryParse(s, out var lng))
                        return lng;
                    if (double.TryParse(s, out var dbl))
                        return dbl;
                    if (bool.TryParse(s, out var b))
                        return b;
                    return s;
                }
                case Dictionary<object, object> d:
                    var e = new ScriptObject();
                    foreach (var (key, value) in d)
                    {
                        e[key.ToString()] = FixTypes(value);
                    }

                    return e;

                case IEnumerable<object> l:
                    return l.Select(FixTypes).ToArray();

                //used by CsvHelper
                case ExpandoObject ex:
                    var dict = ex.ToDictionary(kv =>
                            kv.Key,
                        kv => FixTypes(kv.Value));
                    return ScriptObject.From(dict);
                default:
                    return tree;
            }
        }
    }
}
