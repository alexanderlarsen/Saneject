using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Development.ReadOnlyCollectionDrawer
{
    public class RootScope : Scope
    {
        public override void Configure()
        {
            Bind<ITest, TestMono>().FromDescendants().WhereTransformIsLastSibling();
            Bind<TestMono>().WithId("LOL").FromDescendants().WhereTransformSiblingIndexIs(2);
            Bind<TestMono>().AsGlobal().FromDescendants().WhereTransformSiblingIndexIs(2);
            Bind<ITest, TestMono>().AsCollection().FromDescendants();
            Bind<TestMono>().AsCollection().FromDescendants();
            Bind<Collider>().AsCollection().FromTargetSelf();
        }
    }
}