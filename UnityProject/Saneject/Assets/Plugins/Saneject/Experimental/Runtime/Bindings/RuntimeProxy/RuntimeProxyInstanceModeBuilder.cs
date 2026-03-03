using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Proxy;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.RuntimeProxy
{
    /// <summary>
    /// Builder for configuring the instance mode of a runtime proxy.
    /// </summary>
    public class RuntimeProxyInstanceModeBuilder
    {
        private readonly ComponentBinding binding;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeProxyInstanceModeBuilder"/> class.
        /// </summary>
        /// <param name="binding">The component binding to configure.</param>
        public RuntimeProxyInstanceModeBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        /// <summary>
        /// Sets the proxy instance mode to transient, creating a new instance for each resolution.
        /// </summary>
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

        /// <summary>
        /// Sets the proxy instance mode to singleton, which caches the target instance <see cref="Component"/> in the <see cref="Plugins.Saneject.Experimental.Runtime.Scopes.GlobalScope"/> and reuses it for all resolutions.
        /// </summary>
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