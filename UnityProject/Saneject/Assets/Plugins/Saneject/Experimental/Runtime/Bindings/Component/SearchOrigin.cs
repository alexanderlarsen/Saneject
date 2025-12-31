namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
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