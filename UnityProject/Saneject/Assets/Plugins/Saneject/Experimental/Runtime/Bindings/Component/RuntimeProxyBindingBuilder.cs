using Plugins.Saneject.Experimental.Runtime.Proxy;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
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
                RuntimeProxyResolveMethod.FromGlobalScope,
                prefab: null,
                dontDestroyOnLoad: false
            );
        }

        public void FromAnywhereInLoadedScenes()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                RuntimeProxyResolveMethod.FromAnywhereInLoadedScenes,
                prefab: null,
                dontDestroyOnLoad: false
            );
        }

        public void FromComponentOnPrefab(
            GameObject prefab,
            bool dontDestroyOnLoad)
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                RuntimeProxyResolveMethod.FromComponentOnPrefab,
                prefab,
                dontDestroyOnLoad
            );
        }

        public void FromNewComponentOnNewGameObject(bool dontDestroyOnLoad)
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                RuntimeProxyResolveMethod.FromNewComponentOnNewGameObject,
                prefab: null,
                dontDestroyOnLoad
            );
        }
    }
}