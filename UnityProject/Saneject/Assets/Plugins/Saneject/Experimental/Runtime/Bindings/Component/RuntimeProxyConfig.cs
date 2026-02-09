using Plugins.Saneject.Experimental.Runtime.Proxy;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public class RuntimeProxyConfig
    {
        public RuntimeProxyConfig(
            ProxyResolveMethod resolveMethod,
            GameObject prefab,
            bool dontDestroyOnLoad)
        {
            ResolveMethod = resolveMethod;
            Prefab = prefab;
            DontDestroyOnLoad = dontDestroyOnLoad;
        }

        public ProxyResolveMethod ResolveMethod { get; }
        public GameObject Prefab { get; }
        public bool DontDestroyOnLoad { get; }

        public RuntimeProxyConfig Clone()
        {
            return new RuntimeProxyConfig
            (
                ResolveMethod,
                Prefab,
                DontDestroyOnLoad
            );
        }
    }
}