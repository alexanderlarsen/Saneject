using System.Collections.Generic;
using System.ComponentModel;

namespace Plugins.Saneject.Editor.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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