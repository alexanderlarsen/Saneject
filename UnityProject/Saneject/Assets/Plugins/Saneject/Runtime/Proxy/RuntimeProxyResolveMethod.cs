using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Specifies the strategy a runtime proxy uses to locate or create its target instance at runtime.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum RuntimeProxyResolveMethod
    {
        /// <summary>
        /// Looks up the instance in <see cref="Scopes.GlobalScope"/>.
        /// The target must already be registered there, usually via <see cref="Scopes.Scope.BindGlobal{TConcrete}"/>.
        /// This is the fastest lookup-based resolve method.
        /// </summary>
        FromGlobalScope,

        /// <summary>
        /// Searches all loaded scenes (including inactive objects) for the first instance of the target type using <c>FindFirstObjectByType</c>.
        /// Useful when the target is a scene object that has already been instantiated.
        /// </summary>
        FromAnywhereInLoadedScenes,

        /// <summary>
        /// Instantiates the configured prefab and finds the target component on it.
        /// Requires a prefab to be assigned to the proxy.
        /// </summary>
        FromComponentOnPrefab,

        /// <summary>
        /// Creates a new <see cref="UnityEngine.GameObject"/> and adds the target component to it.
        /// No prefab required; the component is instantiated fresh each time (unless <see cref="RuntimeProxyInstanceMode.Singleton"/> caches it).
        /// </summary>
        FromNewComponentOnNewGameObject
    }
}
