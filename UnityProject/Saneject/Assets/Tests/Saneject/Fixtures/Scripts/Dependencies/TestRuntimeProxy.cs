using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;

namespace Tests.Saneject.Fixtures.Scripts.Dependencies
{
    [MuteUnusedRuntimeProxyWarning]
    public class TestRuntimeProxy : RuntimeProxy<ComponentDependency>, IDependency
    {
    }
}
