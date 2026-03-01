using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AssetFilter : DependencyFilter
    {
        public AssetFilter(
            AssetFilterType filterType,
            Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }

        public AssetFilterType FilterType { get; }
    }
}