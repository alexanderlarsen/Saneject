namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public enum SearchOrigin
    {
        Scope,
        Root,
        InjectionTarget,
        CustomTargetTransform,
        Scene,
        SingleInstance, // Both direct instance and method
        MultipleInstances, // Both direct instance and method
        Proxy
    }
}