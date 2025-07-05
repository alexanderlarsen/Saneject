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
            RegisterComponent<Player>().FromSelf();
            RegisterComponent<Player>().FromParent();
            RegisterComponent<Player>().FromAncestors();
            RegisterComponent<Player>().FromFirstChild();
            RegisterComponent<Player>().FromLastChild();
            RegisterComponent<Player>().FromChildWithIndex(0);
            RegisterComponent<Player>().FromDescendants();
            RegisterComponent<Player>().FromSiblings();

            RegisterComponent<Player>().FromScopeSelf();
            RegisterComponent<Player>().FromScopeParent();
            RegisterComponent<Player>().FromScopeAncestors();
            RegisterComponent<Player>().FromScopeFirstChild();
            RegisterComponent<Player>().FromScopeLastChild();
            RegisterComponent<Player>().FromScopeChildWithIndex(0);
            RegisterComponent<Player>().FromScopeDescendants();
            RegisterComponent<Player>().FromScopeSiblings();

            RegisterComponent<Player>().FromRootSelf();
            RegisterComponent<Player>().FromRootFirstChild();
            RegisterComponent<Player>().FromRootLastChild();
            RegisterComponent<Player>().FromRootChildWithIndex(0);
            RegisterComponent<Player>().FromRootDescendants();

            RegisterComponent<Player>().FromTargetSelf();
            RegisterComponent<Player>().FromTargetParent();
            RegisterComponent<Player>().FromTargetAncestors();
            RegisterComponent<Player>().FromTargetFirstChild();
            RegisterComponent<Player>().FromTargetLastChild();
            RegisterComponent<Player>().FromTargetChildWithIndex(0);
            RegisterComponent<Player>().FromTargetDescendants();
            RegisterComponent<Player>().FromTargetSiblings();

            RegisterComponent<Player>().From(null);
            RegisterComponent<Player>().FromParentOf(null);
            RegisterComponent<Player>().FromAncestorsOf(null);
            RegisterComponent<Player>().FromFirstChildOf(null);
            RegisterComponent<Player>().FromLastChildOf(null);
            RegisterComponent<Player>().FromChildWithIndexOf(null, 0);
            RegisterComponent<Player>().FromDescendantsOf(null);
            RegisterComponent<Player>().FromSiblingsOf(null);

            RegisterComponent<Player>().FromAnywhereInScene();
            RegisterComponent<Player>().FromInstance(null);
            RegisterComponent<Player>().FromMethod(null);

            RegisterComponent<Player>().WithId("").FromMethod(null);
        }
    }
}