using Plugins.Saneject.Legacy.Runtime.Attributes;
using Tests.Legacy.Runtime.Proxy;
using UnityEngine;

namespace Tests.Legacy.Runtime.Component
{
    public partial class ProxyRequester : MonoBehaviour
    {
        [SerializeInterface, Inject]
        public IProxyTarget proxyTarget;
    }
}