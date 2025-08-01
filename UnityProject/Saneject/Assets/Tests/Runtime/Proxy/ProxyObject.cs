using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.InterfaceProxy;

namespace Tests.Runtime.Proxy
{
    [GenerateInterfaceProxy]
    public partial class ProxyObject : InterfaceProxyObject<ProxyTarget>
    {
    }
}