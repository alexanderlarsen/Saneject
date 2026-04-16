using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Implemented by components that hold runtime proxy references and need to swap them with real instances at startup.
    /// </summary>
    /// <remarks>
    /// During scope startup, Saneject calls <see cref="SwapProxiesWithRealInstances"/> for all components registered via <see cref="Scopes.Scope.AddProxySwapTarget"/>.
    /// This allows the component to resolve serialized runtime proxy placeholders and replace them with real runtime instances.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRuntimeProxySwapTarget
    {
        /// <summary>
        /// Called during scope startup to replace serialized proxy references with resolved real instances.
        /// </summary>
        void SwapProxiesWithRealInstances();
    }
}
