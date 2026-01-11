using Plugins.Saneject.Legacy.Runtime.Attributes;
using Plugins.Saneject.Legacy.Runtime.Proxy;

namespace Tests.Legacy.Runtime.Proxy
{
    [GenerateProxyObject]
    public partial class TestProxyObject : ProxyObject<ProxyTarget>
    {
    }
}