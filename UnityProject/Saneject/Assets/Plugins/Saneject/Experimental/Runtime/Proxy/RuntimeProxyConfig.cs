using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    public class RuntimeProxyConfig
    {
        public RuntimeProxyConfig(
            RuntimeProxyResolveMethod resolveMethod,
            GameObject prefab,
            bool dontDestroyOnLoad)
        {
            ResolveMethod = resolveMethod;
            Prefab = prefab;
            DontDestroyOnLoad = dontDestroyOnLoad;
        }

        public RuntimeProxyResolveMethod ResolveMethod { get; }
        public GameObject Prefab { get; }
        public bool DontDestroyOnLoad { get; }
    }
}