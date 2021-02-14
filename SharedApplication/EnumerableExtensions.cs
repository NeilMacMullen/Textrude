using System.Collections.Generic;
using System.Linq;

namespace SharedApplication
{
    /// <summary>
    ///     Simple extensions for IEnumerable
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     return only the non-empty strings and trim them as we go
        /// </summary>
        public static IEnumerable<string> Clean(this IEnumerable<string> items)
        {
            return items.Select(i => i.Trim()).Where(i => i.Length != 0);
        }
    }
}
