using System;
using System.ComponentModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Proxy
{
    /// <summary>
    /// Non-generic base class for all runtime proxy assets.
    /// </summary>
    /// <remarks>
    /// Saneject's editor and runtime systems use this type to work with generated
    /// <see cref="RuntimeProxy{TComponent}"/> assets without needing to know their concrete generic type.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class RuntimeProxyBase : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Play Mode-only resolved instance shown by the runtime proxy inspector.
        /// </summary>
        [NonSerialized, EditorBrowsable(EditorBrowsableState.Never)]
        protected Object resolvedInstance;
#endif
        
        /// <summary>
        /// Determines how the proxy locates or creates the target instance at runtime.
        /// </summary>
        [SerializeField, Tooltip(
             "How this runtime proxy finds or creates its target at runtime.\n\n" +
             "FromGlobalScope: Looks up a component already registered in GlobalScope.\n\n" +
             "FromAnywhereInLoadedScenes: Finds the first matching component in loaded scenes, including inactive objects.\n\n" +
             "FromComponentOnPrefab: Instantiates the configured prefab and resolves the target component on that instance.\n\n" +
             "FromNewComponentOnNewGameObject: Creates a new GameObject and adds the target component.")
        ]
        protected RuntimeProxyResolveMethod resolveMethod;

        /// <summary>
        /// The prefab instantiated by <see cref="RuntimeProxyResolveMethod.FromComponentOnPrefab"/>, then searched for the target component.
        /// </summary>
        [SerializeField, Tooltip("Prefab used by FromComponentOnPrefab. The instantiated object is searched for the target component.")]
        protected GameObject prefab;

        /// <summary>
        /// Whether a creation-based runtime proxy keeps its created object alive across scene loads.
        /// </summary>
        [SerializeField, Tooltip("For creation-based methods, keeps the created object alive across scene loads.")]
        protected bool dontDestroyOnLoad = true;

        /// <summary>
        /// Determines whether a creation-based runtime proxy creates a new instance each time or reuses one created instance.
        /// </summary>
        [SerializeField, Tooltip(
             "Used only by creation-based runtime proxy methods.\n\n" +
             "Transient: Creates a new instance each time the proxy resolves.\n\n" +
             "Singleton: Reuses one created instance and caches it in GlobalScope.")
        ]
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
