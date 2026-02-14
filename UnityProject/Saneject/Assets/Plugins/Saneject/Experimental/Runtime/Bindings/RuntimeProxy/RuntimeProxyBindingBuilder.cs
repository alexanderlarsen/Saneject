using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Proxy;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.RuntimeProxy
{
    public class RuntimeProxyBindingBuilder
    {
        private readonly ComponentBinding binding;

        public RuntimeProxyBindingBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

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

        public RuntimeProxyInstanceModeBuilder FromComponentOnPrefab(
            GameObject prefab,
            bool dontDestroyOnLoad)
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

        public RuntimeProxyInstanceModeBuilder FromNewComponentOnNewGameObject(bool dontDestroyOnLoad)
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