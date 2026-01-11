using Plugins.Saneject.Legacy.Runtime.Scopes;

namespace Tests.Legacy.Runtime.BatchInjection
{
    public class TestScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindComponent<TestDependency>().FromSelf();
        }
    }
}