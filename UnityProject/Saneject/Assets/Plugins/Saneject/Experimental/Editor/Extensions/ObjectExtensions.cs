using System.Collections.Generic;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T obj)
        {
            yield return obj;
        }
        
        public static T[] AsArray<T>(this T obj)
        {
            return new[] { obj };
        }
    }
}