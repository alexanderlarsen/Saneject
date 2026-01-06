using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public class ComponentDependencyFilter : DependencyFilter
    {
        public ComponentFilterType FilterType { get; }
        
        public ComponentDependencyFilter(ComponentFilterType filterType, Func<object, bool> filter) : base(filter)
        {
            FilterType = filterType;
        }
    }
}