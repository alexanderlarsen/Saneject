using System.Collections.Generic;
using System.Linq;

namespace Plugins.Saneject.Legacy.Runtime.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Wrap the <paramref name="item" /> in an <see cref="IEnumerable{T}" /> (empty if null).
        /// </summary>
        public static IEnumerable<T> WrapInEnumerable<T>(this T item)
        {
            return item != null ? new[] { item } : Enumerable.Empty<T>();
        }
    }
}