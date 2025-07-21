using Plugins.Saneject.Runtime.Scopes;

namespace Development.ReadOnlyCollectionDrawer
{
    public class RootScope : Scope
    {
        public override void Configure()
        {
            // Bind<ITest, TestMono>().AsCollection().FromScopeDescendants();
            Bind<ITest, TestMono>().FromScopeDescendants().WhereIsLastSibling();
        }
    }
}