using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromTargetTests
    {
        [Test]
        public void FromTarget_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 2);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 2");

            scope.BindComponent<Dependency>().FromTargetSelf();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetParent_InjectsFromParent()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 2");

            scope.BindComponent<Dependency>().FromTargetParent();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1/Child 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1");

            scope.BindComponent<Dependency>().FromTargetAncestors(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1/Child 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<Dependency>().FromTargetAncestors(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetFirstChild_InjectsFromFirstChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 1");

            scope.BindComponent<Dependency>().FromTargetFirstChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetLastChild_InjectsFromLastChild()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 3");

            scope.BindComponent<Dependency>().FromTargetLastChild();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetChildWithIndex_InjectsFromChildWithIndex()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 2");

            scope.BindComponent<Dependency>().FromTargetChildWithIndex(1);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 1/Child 1");

            scope.BindComponent<Dependency>().FromTargetDescendants(includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 1");

            scope.BindComponent<Dependency>().FromTargetDescendants(includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromTargetSiblings_InjectsFromFirstSibling()
        {
            TestScene scene = TestScene.Create(roots: 1, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 1/Child 2/Child 1");

            scope.BindComponent<Dependency>().FromTargetSiblings();

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}
