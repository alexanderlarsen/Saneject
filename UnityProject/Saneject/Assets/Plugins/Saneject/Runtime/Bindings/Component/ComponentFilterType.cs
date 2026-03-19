using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Bindings.Component
{
    /// <summary>
    /// Specifies the type of filter applied to a component binding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ComponentFilterType
    {
        /// <summary> No filter is applied. </summary>
        None,
        /// <summary> A predicate filter on the component type. </summary>
        Where,
        /// <summary> A predicate filter on the <see cref="UnityEngine.Component"/> instance. </summary>
        WhereComponent,
        /// <summary> A predicate filter on the component's <see cref="UnityEngine.Transform"/>. </summary>
        WhereTransform,
        /// <summary> A predicate filter on the component's <see cref="UnityEngine.GameObject"/>. </summary>
        WhereGameObject,
        /// <summary> A predicate filter on the component's parent <see cref="UnityEngine.Transform"/>. </summary>
        WhereParent,
        /// <summary> A predicate filter on any ancestor of the component's <see cref="UnityEngine.Transform"/>. </summary>
        WhereAnyAncestor,
        /// <summary> A predicate filter on the root of the component's <see cref="UnityEngine.Transform"/> hierarchy. </summary>
        WhereRoot,
        /// <summary> A predicate filter on any child of the component's <see cref="UnityEngine.Transform"/>. </summary>
        WhereAnyChild,
        /// <summary> A predicate filter on a child at a specific index. </summary>
        WhereChildAt,
        /// <summary> A predicate filter on the first child. </summary>
        WhereFirstChild,
        /// <summary> A predicate filter on the last child. </summary>
        WhereLastChild,
        /// <summary> A predicate filter on any descendant of the component's <see cref="UnityEngine.Transform"/>. </summary>
        WhereAnyDescendant,
        /// <summary> A predicate filter on any sibling of the component's <see cref="UnityEngine.Transform"/>. </summary>
        WhereAnySibling
    }
}