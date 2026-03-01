using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ComponentFilterType
    {
        None,
        Where,
        WhereComponent,
        WhereTransform,
        WhereGameObject,
        WhereParent,
        WhereAnyAncestor,
        WhereRoot,
        WhereAnyChild,
        WhereChildAt,
        WhereFirstChild,
        WhereLastChild,
        WhereAnyDescendant,
        WhereAnySibling
    }
}