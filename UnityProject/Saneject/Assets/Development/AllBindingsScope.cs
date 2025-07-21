using Plugins.Saneject.Demo.Scripts.PlayerSystems;
using Plugins.Saneject.Runtime.Scopes;

namespace Development
{
    public class AllBindingsScope : Scope
    {
        public override void Configure()
        {
            ResolveComponents();
        }

        private void ResolveComponents()
        {
            Bind<Player>().FromSelf();
            Bind<Player>().FromParent();
            Bind<Player>().FromAncestors();
            Bind<Player>().FromFirstChild();
            Bind<Player>().FromLastChild();
            Bind<Player>().FromChildWithIndex(0);
            Bind<Player>().FromDescendants();
            Bind<Player>().FromSiblings();

            Bind<Player>().FromScopeSelf();
            Bind<Player>().FromScopeParent();
            Bind<Player>().FromScopeAncestors();
            Bind<Player>().FromScopeFirstChild();
            Bind<Player>().FromScopeLastChild();
            Bind<Player>().FromScopeChildWithIndex(0);
            Bind<Player>().FromScopeDescendants();
            Bind<Player>().FromScopeSiblings();

            Bind<Player>().FromRootSelf();
            Bind<Player>().FromRootFirstChild();
            Bind<Player>().FromRootLastChild();
            Bind<Player>().FromRootChildWithIndex(0);
            Bind<Player>().FromRootDescendants();

            Bind<Player>().FromTargetSelf();
            Bind<Player>().FromTargetParent();
            Bind<Player>().FromTargetAncestors();
            Bind<Player>().FromTargetFirstChild();
            Bind<Player>().FromTargetLastChild();
            Bind<Player>().FromTargetChildWithIndex(0);
            Bind<Player>().FromTargetDescendants();
            Bind<Player>().FromTargetSiblings();

            Bind<Player>().From(null);
            Bind<Player>().FromParentOf(null);
            Bind<Player>().FromAncestorsOf(null);
            Bind<Player>().FromFirstChildOf(null);
            Bind<Player>().FromLastChildOf(null);
            Bind<Player>().FromChildWithIndexOf(null, 0);
            Bind<Player>().FromDescendantsOf(null);
            Bind<Player>().FromSiblingsOf(null);

            Bind<Player>().FromAnywhereInScene();
            Bind<Player>().FromInstance(null);
            Bind<Player>().FromMethod(null);

            Bind<Player>().WithId("").FromMethod(null);
        }
    }
}