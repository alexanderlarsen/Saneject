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

            // BindComponent<ITest>()
            //     .FromDescendants()
            //     .WhereIsLastSibling();

            BindComponent<GameObject>()
                .FromDescendants()
                .WhereIsFirstSibling();

            BindComponent<TestMono>()
                .FromDescendants()
                .WhereIsFirstSibling();

            BindMultipleComponents<ITest>()
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