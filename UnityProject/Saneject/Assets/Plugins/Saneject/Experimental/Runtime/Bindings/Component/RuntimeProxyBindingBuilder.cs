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
                ProxyResolveMethod.FromGlobalScope,
                prefab: null,
                dontDestroyOnLoad: false
            );
        }

        public void FromAnywhereInLoadedScenes()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                ProxyResolveMethod.FromAnywhereInLoadedScenes,
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
                ProxyResolveMethod.FromComponentOnPrefab,
                prefab,
                dontDestroyOnLoad
            );
        }

        public void FromNewComponentOnNewGameObject(bool dontDestroyOnLoad)
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                ProxyResolveMethod.FromNewComponentOnNewGameObject,
                prefab: null,
                dontDestroyOnLoad
            );
        }

        public void FromManualRegistration()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                ProxyResolveMethod.FromManualRegistration,
                prefab: null,
                dontDestroyOnLoad: false
            );
        }
    }
}