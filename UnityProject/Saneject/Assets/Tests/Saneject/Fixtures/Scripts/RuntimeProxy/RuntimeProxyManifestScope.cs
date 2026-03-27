using Plugins.Saneject.Runtime.Scopes;

namespace Tests.Saneject.Fixtures.Scripts.RuntimeProxy
{
    public class RuntimeProxyManifestScope : Scope
    {
        protected override void DeclareBindings()
        {
            BindComponent<IRuntimeProxyTarget, RuntimeProxyTargetComponent>()
                .FromRuntimeProxy();
        }
    }
}
