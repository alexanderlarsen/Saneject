using Plugins.Saneject.Runtime.Scopes;
using Tests.Saneject.Fixtures.Scripts.Dependencies;

namespace Tests.Saneject.Fixtures.Scripts
{
    public class BatchInjectionScope : Scope
    {
        protected override void DeclareBindings()
        {
            BindComponent<ComponentDependency>().FromSelf();
        }
    }
}
