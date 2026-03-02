using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Implemented by components that hold runtime proxy references and need to swap them with real instances at startup.
    /// </summary>
    /// <remarks>
    /// Saneject calls <see cref="SwapProxiesWithRealInstances"/> on Awake for all components registered via <see cref="Scopes.Scope.AddProxySwapTarget"/>.
    /// This allows the component to resolve its proxy references to real instances using the proxy's resolution strategy,
    /// and then replace the serialized proxies with the resolved real instances.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRuntimeProxySwapTarget
    {
        /// <summary>
        /// Called by the Scope on Awake to replace any serialized proxy references with resolved real instances.
        /// </summary>
        void SwapProxiesWithRealInstances();
    }
}