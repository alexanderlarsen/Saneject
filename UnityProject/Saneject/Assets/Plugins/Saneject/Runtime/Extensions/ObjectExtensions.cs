using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="UnityEngine.Object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Wrap the <paramref name="item"/> in an <see cref="IEnumerable{T}"/> (empty if null).
        /// </summary>
        public static IEnumerable<T> WrapInEnumerable<T>(this T item) where T : Object
        {
            return item ? new[] { item } : Enumerable.Empty<T>();
        }
    }
}