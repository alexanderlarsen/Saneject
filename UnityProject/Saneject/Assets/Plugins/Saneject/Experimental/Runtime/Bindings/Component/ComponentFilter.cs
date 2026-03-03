using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    /// <summary>
    /// Represents a filter for component resolution.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ComponentFilter : DependencyFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentFilter"/> class.
        /// </summary>
        /// <param name="filterType">The type of the filter.</param>
        /// <param name="filter">The predicate used to filter the component.</param>
        public ComponentFilter(
            ComponentFilterType filterType,
            Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }

        /// <summary>
        /// Gets the type of the component filter.
        /// </summary>
        public ComponentFilterType FilterType { get; }
    }
}