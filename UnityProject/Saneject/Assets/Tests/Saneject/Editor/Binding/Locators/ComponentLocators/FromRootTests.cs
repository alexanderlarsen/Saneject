using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Editor.Utilities;
using Tests.Saneject.Fixtures.Scripts;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromRootTests
    {
        [Test]
        public void FromRootSelf_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1");

            scope.BindComponent<Dependency>().FromRootSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromRootFirstChild_InjectsFromFirstChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 1");

            scope.BindComponent<Dependency>().FromRootFirstChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromRootLastChild_InjectsFromLastChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 3");

            scope.BindComponent<Dependency>().FromRootLastChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromRootChildWithIndex_InjectsFromChildWithIndex()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 2");

            scope.BindComponent<Dependency>().FromRootChildWithIndex(1);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromRootDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<Dependency>().FromRootDescendants(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromRootDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1/Child 2/Child 3");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1");

            scope.BindComponent<Dependency>().FromRootDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}