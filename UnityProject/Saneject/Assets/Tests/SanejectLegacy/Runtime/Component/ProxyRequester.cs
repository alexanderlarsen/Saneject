using Plugins.SanejectLegacy.Runtime.Attributes;
using Tests.SanejectLegacy.Runtime.Proxy;
using UnityEngine;

namespace Tests.SanejectLegacy.Runtime.Component
{
    public partial class ProxyRequester : MonoBehaviour
    {
        [SerializeInterface, Inject]
        public IProxyTarget proxyTarget;
    }
}