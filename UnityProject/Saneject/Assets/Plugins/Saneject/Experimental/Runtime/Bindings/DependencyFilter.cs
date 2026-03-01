using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class DependencyFilter
    {
        protected DependencyFilter(Func<object, bool> filter)
        {
            Filter = filter;
        }

        public Func<object, bool> Filter { get; }
    }
}