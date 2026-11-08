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

    [MuteUnusedRuntimeProxyWarning]
    public class MutedUnusedRuntimeProxy : RuntimeProxy<UnusedProxyComponent>
    {
    }
}
