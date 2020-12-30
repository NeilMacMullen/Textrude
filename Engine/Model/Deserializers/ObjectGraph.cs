using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
                    if (int.TryParse(s, out var n))
                        return n;
                    if (bool.TryParse(s, out var b))
                        return b;
                    return s;
                }
                case Dictionary<object, object> d:
                    var e = new Dictionary<string, object>();
                    foreach (var (key, value) in d)
                    {
                        e[key.ToString()] = FixTypes(value);
                    }

                    return e;

                case IEnumerable<object> l:
                    return l.Select(FixTypes).ToArray();

                //used by CsvHelper
                case ExpandoObject ex:
                    return ex.ToDictionary(kv =>
                            kv.Key,
                        kv => FixTypes(kv.Value));
                default:
                    return tree;
            }
        }
    }
}