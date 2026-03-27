using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    public class MissingProxyStubComponent : MonoBehaviour
    {
    }

    public class UnusedProxyComponent : MonoBehaviour
    {
    }

    public class UnusedRuntimeProxy : RuntimeProxy<UnusedProxyComponent>
    {
    }

    [MuteUnusedRuntimeProxyWarning]
    public class MutedUnusedRuntimeProxy : RuntimeProxy<UnusedProxyComponent>
    {
    }

    public class RuntimeProxyAssetReferenceHolder : ScriptableObject
    {
        public RuntimeProxyBase proxy;
    }
}
