using System.ComponentModel;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    /// <summary>
    /// Encapsulates the resolution strategy and instance configuration for a runtime proxy.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RuntimeProxyConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeProxyConfig"/> class.
        /// </summary>
        /// <param name="resolveMethod">The strategy the proxy will use to locate or create the target instance.</param>
        /// <param name="instanceMode">Whether to create a new instance each time or cache a singleton.</param>
        /// <param name="prefab">The prefab to instantiate (required only if <paramref name="resolveMethod"/> is <see cref="RuntimeProxyResolveMethod.FromComponentOnPrefab"/>).</param>
        /// <param name="dontDestroyOnLoad">Whether the resolved instance should survive scene transitions.</param>
        public RuntimeProxyConfig(
            RuntimeProxyResolveMethod resolveMethod,
            RuntimeProxyInstanceMode instanceMode,
            GameObject prefab,
            bool dontDestroyOnLoad)
        {
            ResolveMethod = resolveMethod;
            InstanceMode = instanceMode;
            Prefab = prefab;
            DontDestroyOnLoad = dontDestroyOnLoad;
        }

        /// <summary>
        /// Gets the strategy for locating or creating the target instance.
        /// </summary>
        public RuntimeProxyResolveMethod ResolveMethod { get; }

        /// <summary>
        /// Gets the instance creation mode (singleton or transient).
        /// </summary>
        public RuntimeProxyInstanceMode InstanceMode { get; }

        /// <summary>
        /// Gets the prefab to instantiate, if applicable.
        /// </summary>
        public GameObject Prefab { get; }

        /// <summary>
        /// Gets a value indicating whether the resolved instance survives scene transitions.
        /// </summary>
        public bool DontDestroyOnLoad { get; }
    }
}