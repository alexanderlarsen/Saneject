using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Bindings.Component
{
    /// <summary>
    /// Specifies the direction to search for a component relative to a search origin.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SearchDirection
    {
        /// <summary> No search direction is specified. </summary>
        None,

        /// <summary> Search only on the origin object itself. </summary>
        Self,

        /// <summary> Search on the immediate parent object. </summary>
        Parent,

        /// <summary> Search upwards through all ancestor objects. </summary>
        Ancestors,

        /// <summary> Search on the first direct child object. </summary>
        FirstChild,

        /// <summary> Search on the last direct child object. </summary>
        LastChild,

        /// <summary> Search on the child object at a specific index. </summary>
        ChildAtIndex,

        /// <summary> Search downwards through all descendant objects. </summary>
        Descendants,

        /// <summary> Search on all sibling objects (other children of the same parent). </summary>
        Siblings,

        /// <summary> Search anywhere in the scene. </summary>
        Anywhere
    }
}