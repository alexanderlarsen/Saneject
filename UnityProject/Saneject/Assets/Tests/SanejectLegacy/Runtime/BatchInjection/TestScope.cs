using Plugins.SanejectLegacy.Runtime.Scopes;

namespace Tests.SanejectLegacy.Runtime.BatchInjection
{
    public class TestScope : Scope
    {
        public override void ConfigureBindings()
        {
            BindComponent<TestDependency>().FromSelf();
        }
    }
}