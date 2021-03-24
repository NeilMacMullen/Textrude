using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Application
{
    public class Group
    {
        private readonly Dictionary<object, List<object>> _data = new Dictionary<object, List<object>>();

        public void Add(object key, object item)
        {
            if (!_data.TryGetValue(key, out var lst))
            {
                lst = new List<object>();
                _data[key] = lst;
            }

            lst.Add(item);
        }

        public object[] Flatten()
        {
            return _data.SelectMany(l => l.Value).ToArray();
        }

        public static Group Create() => new Group();

        public static void Add(Group grp, object key, object val)
        {
            if (grp == null || key == null || val == null)
                return;
            grp.Add(key, val);
        }

        public static void AddMany(Group grp, object key, object val)
        {
            if (grp == null || key == null || val == null)
                return;
            if (val is IEnumerable<object> arr)
                foreach (var a in arr)
                    grp.Add(key, a);
        }

        public static object[] Flatten(Group grp)
        {
            if (grp == null)
                return Array.Empty<object>();
            return grp.Flatten();
        }

        public static object[] ToKvArray(Group grp)
        {
            if (grp == null)
                return Array.Empty<object>();
            return grp._data.Select(
                kv => (object) new Dictionary<string, object>
                {
                    ["key"] = kv.Key,
                    ["values"] = (object) kv.Value.ToArray()
                }
            ).ToArray();
        }
    }
}
