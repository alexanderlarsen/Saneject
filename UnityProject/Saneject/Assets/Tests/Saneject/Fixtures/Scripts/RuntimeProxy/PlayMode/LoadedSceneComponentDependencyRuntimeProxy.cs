using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Proxy;
using Tests.Saneject.Fixtures.Scripts.Dependencies;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy.PlayMode
{
    [MuteUnusedRuntimeProxyWarning]
    public class LoadedSceneComponentDependencyRuntimeProxy : RuntimeProxy<LoadedSceneComponentDependency>, IDependency
    {
    }
}
