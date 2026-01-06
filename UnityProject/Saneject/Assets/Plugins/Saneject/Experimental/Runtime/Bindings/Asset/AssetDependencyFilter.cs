using System;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    public class AssetDependencyFilter : DependencyFilter
    {
        public AssetDependencyFilter(
            AssetFilterType filterType,
            Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }

        public AssetFilterType FilterType { get; }
    }
}