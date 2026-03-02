using System.ComponentModel;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for all <see cref="RuntimeProxy{TConcrete}"/> proxies.
    /// </summary>
    /// <remarks>
    /// This base class is required so that the Saneject editor and runtime systems can work with proxy assets
    /// without needing to know the specific generic type. All <see cref="RuntimeProxy{TConcrete}"/> instances inherit
    /// from this base and can be accessed as <see cref="RuntimeProxyBase"/>.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RuntimeProxyBase : ScriptableObject
    {
        /// <summary>
        /// Determines how the proxy locates or creates the target instance at runtime.
        /// </summary>
        [SerializeField, Tooltip(
             "Determines how the proxy locates or creates the target instance at runtime.\n\n" +
             "FromGlobalScope: Uses GlobalScopeRegistry. Instance must be registered using RegisterGlobalComponent or RegisterGlobalObject in a scope.\n\n" +
             "FindInLoadedScenes: Finds the first matching component in any loaded scene using FindFirstObjectByType<TConcrete>(FindObjectsInactive.Include).\n\n" +
             "FromComponentOnPrefab: Instantiates a prefab and finds the target component.\n\n" +
             "FromNewComponentOnNewGameObject: Creates a new GameObject and adds the component.")
        ]
        protected RuntimeProxyResolveMethod resolveMethod;

        /// <summary>
        /// The prefab which to instantiate and get the target component from (used by <see cref="RuntimeProxyResolveMethod.FromComponentOnPrefab"/>).
        /// </summary>
        [SerializeField, Tooltip("The prefab which to instantiate and get the target component from.")]
        protected GameObject prefab;

        /// <summary>
        /// If <c>true</c>, the resolved instance will not be destroyed when loading a new scene.
        /// </summary>
        [SerializeField, Tooltip("Do not destroy the target Object when loading a new Scene.")]
        protected bool dontDestroyOnLoad = true;

        /// <summary>
        /// Determines whether the proxy creates a new instance each time or caches a singleton instance.
        /// </summary>
        [SerializeField]
        protected RuntimeProxyInstanceMode instanceMode;

        /// <summary>
        /// Resolves and returns the real instance for this proxy.
        /// </summary>
        /// <returns>The resolved component instance, or null if resolution fails.</returns>
        public abstract object ResolveInstance();

        /// <summary>
        /// Applies the resolution strategy and instance configuration from the specified config object.
        /// </summary>
        /// <param name="config">The configuration to apply.</param>
        public void AssignConfig(RuntimeProxyConfig config)
        {
            resolveMethod = config.ResolveMethod;
            prefab = config.Prefab;
            dontDestroyOnLoad = config.DontDestroyOnLoad;
            instanceMode = config.InstanceMode;
        }

        /// <summary>
        /// Determines whether this proxy has the same configuration as the specified config object.
        /// </summary>
        /// <param name="config">The configuration to compare against.</param>
        /// <returns><c>true</c> if all resolution settings match; otherwise, <c>false</c>.</returns>
        public bool HasConfig(RuntimeProxyConfig config)
        {
            return resolveMethod == config.ResolveMethod &&
                   prefab == config.Prefab &&
                   dontDestroyOnLoad == config.DontDestroyOnLoad &&
                   instanceMode == config.InstanceMode;
        }
    }
}