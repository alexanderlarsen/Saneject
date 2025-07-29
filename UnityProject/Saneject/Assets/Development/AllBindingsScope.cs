using System;
using Plugins.Saneject.Demo.Scripts.PlayerSystems;
using Plugins.Saneject.Runtime.Scopes;

namespace Development
{
    public class AllBindingsScope : Scope
    {
        protected override void ConfigureBindings()
        {
            BindComponent<Player>().FromSelf();
            BindComponent<Player>().FromParent();
            BindComponent<Player>().FromAncestors();
            BindComponent<Player>().FromFirstChild();
            BindComponent<Player>().FromLastChild();
            BindComponent<Player>().FromChildWithIndex(0);
            BindComponent<Player>().FromDescendants();
            BindComponent<Player>().FromSiblings();

            BindComponent<Player>().FromScopeSelf();
            BindComponent<Player>().FromScopeParent();
            BindComponent<Player>().FromScopeAncestors();
            BindComponent<Player>().FromScopeFirstChild();
            BindComponent<Player>().FromScopeLastChild();
            BindComponent<Player>().FromScopeChildWithIndex(0);
            BindComponent<Player>().FromScopeDescendants();
            BindComponent<Player>().FromScopeSiblings();

            BindComponent<Player>().FromRootSelf();
            BindComponent<Player>().FromRootFirstChild();
            BindComponent<Player>().FromRootLastChild();
            BindComponent<Player>().FromRootChildWithIndex(0);
            BindComponent<Player>().FromRootDescendants();

            BindComponent<Player>().FromTargetSelf();
            BindComponent<Player>().FromTargetParent();
            BindComponent<Player>().FromTargetAncestors();
            BindComponent<Player>().FromTargetFirstChild();
            BindComponent<Player>().FromTargetLastChild();
            BindComponent<Player>().FromTargetChildWithIndex(0);
            BindComponent<Player>().FromTargetDescendants();
            BindComponent<Player>().FromTargetSiblings();

            BindComponent<Player>().From(null);
            BindComponent<Player>().FromParentOf(null);
            BindComponent<Player>().FromAncestorsOf(null);
            BindComponent<Player>().FromFirstChildOf(null);
            BindComponent<Player>().FromLastChildOf(null);
            BindComponent<Player>().FromChildWithIndexOf(null, 0);
            BindComponent<Player>().FromDescendantsOf(null);
            BindComponent<Player>().FromSiblingsOf(null);

            BindComponent<Player>().FromAnywhereInScene();
            BindComponent<Player>().FromInstance(null);
            BindComponent<Player>().FromMethod((Func<Player>)null);

            BindComponent<Player>().WithId("").FromMethod((Func<Player>)null);
        }
    }
}