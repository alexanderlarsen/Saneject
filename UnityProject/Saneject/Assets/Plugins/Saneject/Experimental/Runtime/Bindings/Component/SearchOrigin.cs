using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    /// <summary>
    /// Specifies the origin object for a component search.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SearchOrigin
    {
        /// <summary> The search starts from the <see cref="Plugins.Saneject.Experimental.Runtime.Scopes.Scope" /> object. </summary>
        Scope,

        /// <summary> The search starts from the root of the <see cref="Plugins.Saneject.Experimental.Runtime.Scopes.Scope" /> hierarchy. </summary>
        Root,

        /// <summary> The search starts from the object being injected into. </summary>
        InjectionTarget,

        /// <summary> The search starts from a specified custom <see cref="UnityEngine.Transform" />. </summary>
        CustomTargetTransform,

        /// <summary> The search is performed scene-wide. </summary>
        Scene,

        /// <summary> The search is performed using a direct instance or a method. </summary>
        Instance
    }
}