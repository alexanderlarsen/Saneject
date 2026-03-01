using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ComponentFilter : DependencyFilter
    {
        public ComponentFilter(
            ComponentFilterType filterType,
            Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }

        public ComponentFilterType FilterType { get; }
    }
}