using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public class RootScope : Scope
    {
        protected override void ConfigureBindings()
        {
            BindGlobal<TestMono>()
                .FromDescendants()
                .WhereSiblingIndexIs(2);

            BindComponent<ITest, TestMono>()
                .FromDescendants()
                .WhereIsLastSibling();

            BindComponent<TestMono>()
                .WithId("LOL")
                .FromDescendants()
                .WhereSiblingIndexIs(2);

            BindMultipleComponents<ITest, TestMono>()
                .FromDescendants();

            BindMultipleComponents<TestMono>()
                .FromDescendants();

            BindMultipleComponents<Collider>()
                .FromTargetSelf();
            
            
        }
    }
}