using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class EnumerableOverlapExtensions
    {
        public static bool OverlapsWith(
            this IEnumerable<Type> a,
            IEnumerable<Type> b)
        {
            List<Type> aList = a.ToList();
            List<Type> bList = b.ToList();

            foreach (Type t1 in aList)
                foreach (Type t2 in bList)
                    if (t1.IsAssignableFrom(t2) || t2.IsAssignableFrom(t1))
                        return true;

            return false;
        }

        public static bool OverlapsWith(
            this IEnumerable<string> a,
            IEnumerable<string> b)
        {
            HashSet<string> aSet = new(a.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
            HashSet<string> bSet = new(b.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);

            foreach (string id in aSet)
                if (bSet.Contains(id))
                    return true;

            return false;
        }
    }
}