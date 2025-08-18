using Plugins.Saneject.Runtime.Attributes;
using Tests.Runtime.Proxy;
using UnityEngine;

namespace Tests.Runtime.Component
{
    public partial class ProxyRequester : MonoBehaviour
    {
        [SerializeInterface, Inject]
        public IProxyTarget proxyTarget;
    }
}