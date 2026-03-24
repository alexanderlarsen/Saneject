using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromScopeTests
    {
        [Test]
        public void FromScopeSelf_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeParent_InjectsFromParent()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 2);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1/Child 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromParent();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromAncestors(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToRoots<ComponentDependency>();
            scene.AddToLeafs<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1/Child 1/Child 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromAncestors(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeFirstChild_InjectsFromFirstChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromFirstChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeLastChild_InjectsFromLastChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 3");

            scope.BindComponent<ComponentDependency>().FromLastChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeChildWithIndex_InjectsFromChildWithIndex()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 2");

            scope.BindComponent<ComponentDependency>().FromChildWithIndex(1);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToLeafs<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromDescendants(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToRoots<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1");

            scope.BindComponent<ComponentDependency>().FromDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromScopeSiblings_InjectsFromFirstSibling()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddToAllTransforms<ComponentDependency>();

            TestScope scope = scene.Add<TestScope>("Root 1/Child 2");
            ConcreteTarget target = scene.Add<ConcreteTarget>("Root 1/Child 2");
            ComponentDependency expected = scene.Get<ComponentDependency>("Root 1/Child 1");

            scope.BindComponent<ComponentDependency>().FromSiblings();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}
