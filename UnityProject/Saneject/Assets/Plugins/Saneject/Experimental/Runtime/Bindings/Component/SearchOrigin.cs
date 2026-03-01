using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SearchOrigin
    {
        Scope,
        Root,
        InjectionTarget,
        CustomTargetTransform,
        Scene,
        Instance, // Both direct instance and method 
    }
}