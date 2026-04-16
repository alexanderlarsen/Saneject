using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Proxy;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Bindings.RuntimeProxy
{
    /// <summary>
    /// Builder for configuring runtime proxy resolution strategies.
    /// </summary>
    public class RuntimeProxyBindingBuilder
    {
        private readonly ComponentBinding binding;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeProxyBindingBuilder"/> class.
        /// </summary>
        /// <param name="binding">The component binding to configure.</param>
        public RuntimeProxyBindingBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        /// <summary>
        /// Resolves the target <see cref="Component"/> from <see cref="GlobalScope"/>.
        /// The target must already be registered there, usually via a global component binding.
        /// </summary>
        public void FromGlobalScope()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromGlobalScope,
                instanceMode: RuntimeProxyInstanceMode.Transient, // lookup-only
                prefab: null,
                dontDestroyOnLoad: false
            );
        }

        /// <summary>
        /// Resolves the target <see cref="Component"/> by finding the first instance of that type in any loaded scene.
        /// Includes inactive objects in the search.
        /// </summary>
        public void FromAnywhereInLoadedScenes()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: false
            );
        }

        /// <summary>
        /// Resolves the proxy by instantiating the specified prefab and getting the target <see cref="Component"/> from it.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="dontDestroyOnLoad">Whether to apply DontDestroyOnLoad to the instantiated object.</param>
        /// <returns>A <see cref="RuntimeProxyInstanceModeBuilder"/> to configure the instance mode.</returns>
        public RuntimeProxyInstanceModeBuilder FromComponentOnPrefab(
            GameObject prefab,
            bool dontDestroyOnLoad = false)
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromComponentOnPrefab,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: prefab,
                dontDestroyOnLoad: dontDestroyOnLoad
            );

            return new RuntimeProxyInstanceModeBuilder(binding);
        }

        /// <summary>
        /// Resolves the target <see cref="Component"/> by creating a new <see cref="GameObject"/> and adding the <see cref="Component"/> to it.
        /// </summary>
        /// <param name="dontDestroyOnLoad">Whether to apply DontDestroyOnLoad to the new object.</param>
        /// <returns>A <see cref="RuntimeProxyInstanceModeBuilder"/> to configure the instance mode.</returns>
        public RuntimeProxyInstanceModeBuilder FromNewComponentOnNewGameObject(bool dontDestroyOnLoad = false)
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: null,
                dontDestroyOnLoad: dontDestroyOnLoad
            );

            return new RuntimeProxyInstanceModeBuilder(binding);
        }
    }
}
