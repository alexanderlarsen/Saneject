using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Tests.Runtime.Proxy
{
    [GenerateProxyObject]
    public partial class TestProxyObject : ProxyObject<ProxyTarget>
    {
    }
}