using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public class RootScope : Scope
    {
        protected override void ConfigureBindings()
        {
            // BindGlobal<TestMono>()
            //     .FromDescendants()
            //     .WhereSiblingIndexIs(2);

            // BindComponent<ITest, TestMono>()
            //     .FromDescendants()
            //     .WhereIsLastSibling();

            BindComponent<TestMono>()
                .FromDescendants()
                .WhereIsFirstSibling();

            BindComponent<ITest>()
                .FromDescendants()
                .WhereIsLastSibling();

            BindComponent<ITest, TestMono>()
                .FromDescendants()
                .WhereIsLastSibling();

            BindMultipleComponents<ITest>()
                .FromDescendants();

            BindMultipleComponents<ITest, TestMono>()
                .FromDescendants()
                .Where(t => t is Component c && c.transform.GetSiblingIndex() % 2 == 0);

            // BindComponent<TestMono>()
            //     .WithId("LOL")
            //     .FromDescendants()
            //     .WhereSiblingIndexIs(2);

            // BindMultipleComponents<ITest, TestMono>()
            //     .FromDescendants();
            //
            // BindMultipleComponents<TestMono>()
            //     .FromDescendants();
            //
            // BindMultipleComponents<Collider>()
            //     .FromTargetSelf();
        }
    }
}