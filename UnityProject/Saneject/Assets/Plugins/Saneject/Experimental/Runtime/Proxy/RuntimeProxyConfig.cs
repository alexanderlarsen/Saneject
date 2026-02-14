using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Proxy
{
    public class RuntimeProxyConfig
    {
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

        public RuntimeProxyResolveMethod ResolveMethod { get; }
        public RuntimeProxyInstanceMode InstanceMode { get; }
        public GameObject Prefab { get; }
        public bool DontDestroyOnLoad { get; }
    }
}