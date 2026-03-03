using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    /// <summary>
    /// Base class for defining custom filters for dependency resolution.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class DependencyFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyFilter"/> class.
        /// </summary>
        /// <param name="filter">The predicate used to filter dependencies.</param>
        protected DependencyFilter(Func<object, bool> filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Gets the predicate used to filter dependencies.
        /// </summary>
        public Func<object, bool> Filter { get; }
    }
}