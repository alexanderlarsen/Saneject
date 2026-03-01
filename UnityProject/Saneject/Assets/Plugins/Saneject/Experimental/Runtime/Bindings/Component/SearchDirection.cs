using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SearchDirection
    {
        None,
        Self,
        Parent,
        Ancestors,
        FirstChild,
        LastChild,
        ChildAtIndex,
        Descendants,
        Siblings,
        Anywhere
    }
}