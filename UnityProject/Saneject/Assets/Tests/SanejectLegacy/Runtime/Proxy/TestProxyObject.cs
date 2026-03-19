using Plugins.SanejectLegacy.Runtime.Attributes;
using Plugins.SanejectLegacy.Runtime.Proxy;

namespace Tests.SanejectLegacy.Runtime.Proxy
{
    [GenerateProxyObject]
    public partial class TestProxyObject : ProxyObject<ProxyTarget>
    {
    }
}