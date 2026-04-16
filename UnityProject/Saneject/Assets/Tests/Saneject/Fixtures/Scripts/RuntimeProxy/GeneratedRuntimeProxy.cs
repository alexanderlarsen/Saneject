using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    [GenerateRuntimeProxy]
    public partial class GeneratedRuntimeProxy : RuntimeProxy<RuntimeProxyTargetComponent>
    {
    }
}
