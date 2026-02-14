using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Proxy;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.RuntimeProxy
{
    public class RuntimeProxyInstanceModeBuilder
    {
        private readonly ComponentBinding binding;

        public RuntimeProxyInstanceModeBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        public void AsTransient()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: binding.RuntimeProxyConfig.ResolveMethod,
                instanceMode: RuntimeProxyInstanceMode.Transient,
                prefab: binding.RuntimeProxyConfig.Prefab,
                dontDestroyOnLoad: binding.RuntimeProxyConfig.DontDestroyOnLoad
            );
        }

        public void AsSingleton()
        {
            binding.RuntimeProxyConfig = new RuntimeProxyConfig
            (
                resolveMethod: binding.RuntimeProxyConfig.ResolveMethod,
                instanceMode: RuntimeProxyInstanceMode.Singleton,
                prefab: binding.RuntimeProxyConfig.Prefab,
                dontDestroyOnLoad: true
            );
        }
    }
}