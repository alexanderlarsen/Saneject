using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    /// <summary>
    /// Represents a filter for asset resolution.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AssetFilter : DependencyFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFilter"/> class.
        /// </summary>
        /// <param name="filterType">The type of the filter.</param>
        /// <param name="filter">The delegate used to filter the asset.</param>
        public AssetFilter(
            AssetFilterType filterType,
            Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }

        /// <summary>
        /// Gets the type of the asset filter.
        /// </summary>
        public AssetFilterType FilterType { get; }
    }
}