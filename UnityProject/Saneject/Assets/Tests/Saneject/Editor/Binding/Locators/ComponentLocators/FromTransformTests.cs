using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Editor.Utilities;
using Tests.Saneject.Fixtures.Scripts;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromTransformTests
    {
        [Test]
        public void From_InjectsFromTransform()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 3/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 3/Child 3");

            scope.BindComponent<Dependency>().From(transform);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromParentOf_InjectsFromParentOfTransform()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 3/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 3");

            scope.BindComponent<Dependency>().FromParentOf(transform);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromAncestorsOf_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 1/Child 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 2");

            scope.BindComponent<Dependency>().FromAncestorsOf(transform, includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromAncestorsOf_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1/Child 1");
            Transform transform = scene.GetTransform("Root 2/Child 1/Child 1");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 1/Child 1");

            scope.BindComponent<Dependency>().FromAncestorsOf(transform, includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromFirstChildOf_InjectsFromFirstChild()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 1");

            scope.BindComponent<Dependency>().FromFirstChildOf(transform);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromLastChildOf_InjectsFromLastChild()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 3");

            scope.BindComponent<Dependency>().FromLastChildOf(transform);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromChildWithIndexOf_InjectsFromChildWithIndex()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 2");

            scope.BindComponent<Dependency>().FromChildWithIndexOf(transform, 1);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 1/Child 1");

            scope.BindComponent<Dependency>().FromDescendantsOf(transform, includeSelf: false);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromDescendantsOf_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToRoots();
            scene.AddDependenciesToLeafs();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2");
            Dependency expected = scene.GetComponent<Dependency>("Root 2");

            scope.BindComponent<Dependency>().FromDescendantsOf(transform, includeSelf: true);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }

        [Test]
        public void FromSiblingsOf_InjectsFromFirstSibling()
        {
            TestScene scene = TestScene.Create(roots: 2, width: 3, depth: 3);
            scene.AddDependenciesToAllTransforms();

            TestScope scope = scene.AddComponent<TestScope>("Root 1");
            ConcreteTarget target = scene.AddComponent<ConcreteTarget>("Root 1");
            Transform transform = scene.GetTransform("Root 2/Child 2/Child 3");
            Dependency expected = scene.GetComponent<Dependency>("Root 2/Child 2/Child 1");

            scope.BindComponent<Dependency>().FromSiblingsOf(transform);

            InjectionRunner.Run(scene.Roots, ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expected, target.dependency);
        }
    }
}
