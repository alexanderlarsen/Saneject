using System;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    public abstract class DependencyFilter
    {
        protected DependencyFilter(Func<object, bool> filter)
        {
            Filter = filter;
        }

        public Func<object, bool> Filter { get; }
        
    }
}