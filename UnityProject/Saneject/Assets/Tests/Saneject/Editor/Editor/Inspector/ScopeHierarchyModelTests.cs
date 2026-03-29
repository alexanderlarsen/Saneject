using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Inspectors.Models;
using Tests.Saneject.Fixtures.Scripts;

namespace Tests.Saneject.Editor.Editor.Inspector
{
    public class ScopeHierarchyModelTests
    {
        [Test]
        public void ScopeHierarchyModel_GivenNestedScopes_CapturesNearestDescendantScopeHierarchy()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 2, depth: 4);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope nestedScope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            TestScope deepNestedScope = scene.Add<TestScope>("Root 1/Child 1/Child 1/Child 1");
            TestScope siblingScope = scene.Add<TestScope>("Root 1/Child 2");
            ContextIdentity inspectedScopeContextIdentity = new(nestedScope);

            // Capture model
            ScopeHierarchyModel model = new(rootScope, nestedScope, inspectedScopeContextIdentity);

            // Assert
            Assert.That(model.ScopeName, Is.EqualTo(nameof(TestScope)));
            Assert.That(model.GameObject, Is.EqualTo(rootScope.gameObject));
            Assert.That(model.IsCurrent, Is.False);
            Assert.That(model.IsSameContext, Is.True);
            Assert.That(model.ContextIdentity, Is.EqualTo(new ContextIdentity(rootScope)));
            Assert.That(model.Children, Has.Count.EqualTo(2));

            Assert.That(model.Children[0].GameObject, Is.EqualTo(nestedScope.gameObject));
            Assert.That(model.Children[0].IsCurrent, Is.True);
            Assert.That(model.Children[0].IsSameContext, Is.True);
            Assert.That(model.Children[0].Children, Has.Count.EqualTo(1));
            Assert.That(model.Children[0].Children[0].GameObject, Is.EqualTo(deepNestedScope.gameObject));
            Assert.That(model.Children[0].Children[0].IsCurrent, Is.False);
            Assert.That(model.Children[0].Children[0].IsSameContext, Is.True);

            Assert.That(model.Children[1].GameObject, Is.EqualTo(siblingScope.gameObject));
            Assert.That(model.Children[1].IsCurrent, Is.False);
            Assert.That(model.Children[1].IsSameContext, Is.True);
            Assert.That(model.Children[1].Children, Is.Empty);
        }

        [Test]
        public void ScopeHierarchyModel_GivenMixedSceneAndPrefabInstanceScopes_SetsCurrentAndContextFlags()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            TestScope rootScope = scene.Add<TestScope>("Root 1");
            TestScope sceneChildScope = scene.Add<TestScope>("Root 1/Child 1");
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 2);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            TestScope prefabScope = prefabInstance.Add<TestScope>("Prefab Root");
            TestScope nestedPrefabScope = prefabInstance.Add<TestScope>("Prefab Root/Child 1");
            ContextIdentity inspectedScopeContextIdentity = new(prefabScope);

            try
            {
                // Capture model
                ScopeHierarchyModel model = new(rootScope, prefabScope, inspectedScopeContextIdentity);

                // Assert
                Assert.That(model.GameObject, Is.EqualTo(rootScope.gameObject));
                Assert.That(model.IsCurrent, Is.False);
                Assert.That(model.IsSameContext, Is.False);
                Assert.That(model.Children, Has.Count.EqualTo(2));

                Assert.That(model.Children[0].GameObject, Is.EqualTo(sceneChildScope.gameObject));
                Assert.That(model.Children[0].IsCurrent, Is.False);
                Assert.That(model.Children[0].IsSameContext, Is.False);

                Assert.That(model.Children[1].GameObject, Is.EqualTo(prefabScope.gameObject));
                Assert.That(model.Children[1].IsCurrent, Is.True);
                Assert.That(model.Children[1].IsSameContext, Is.True);
                Assert.That(model.Children[1].Children, Has.Count.EqualTo(1));
                Assert.That(model.Children[1].Children[0].GameObject, Is.EqualTo(nestedPrefabScope.gameObject));
                Assert.That(model.Children[1].Children[0].IsCurrent, Is.False);
                Assert.That(model.Children[1].Children[0].IsSameContext, Is.True);
            }
            finally
            {
                prefabInstance?.Destroy();

                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}
