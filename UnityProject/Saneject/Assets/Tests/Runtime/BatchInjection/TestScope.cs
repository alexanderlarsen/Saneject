using Plugins.Saneject.Runtime.Scopes;

namespace Tests.Runtime.BatchInjection
{
    public class TestScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindComponent<TestDependency>().FromSelf();
        }
    }
}