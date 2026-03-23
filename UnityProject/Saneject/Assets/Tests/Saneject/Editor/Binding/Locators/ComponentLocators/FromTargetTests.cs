using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Editor.Extensions;
using Tests.Saneject.Runtime.Fixtures;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromTargetTests
    {
        private const string LocatorScenePath = "Assets/Tests/Saneject/Runtime/Fixtures/LocatorScene.unity";

        private Scene scene;

        [SetUp]
        public void SetUp()
        {
            scene = EditorSceneManager.OpenScene(LocatorScenePath, OpenSceneMode.Single);
        }

        [Test]
        public void FromTarget_InjectsFromSelf()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2";
            const string expectedDependencyPath = "Root 1/Child 2";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetSelf();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetParent_InjectsFromParent()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2/GrandChild 1";
            const string expectedDependencyPath = "Root 1/Child 2";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetParent();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2/GrandChild 1";
            const string expectedDependencyPath = "Root 1/Child 2/";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetAncestors(includeSelf: false);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetAncestors_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2/GrandChild 1";
            const string expectedDependencyPath = "Root 1/Child 2/GrandChild 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetAncestors(includeSelf: true);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetFirstChild_InjectsFromFirstChild()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2";
            const string expectedDependencyPath = "Root 1/Child 2/GrandChild 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetFirstChild();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetLastChild_InjectsFromLastChild()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2";
            const string expectedDependencyPath = "Root 1/Child 2/GrandChild 3";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetLastChild();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetChildWithIndex_InjectsFromChildWithIndex()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2";
            const string expectedDependencyPath = "Root 1/Child 2/GrandChild 2";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetChildWithIndex(1);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1";
            const string expectedDependencyPath = "Root 1/Child 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetDescendants(includeSelf: false);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1";
            const string expectedDependencyPath = "Root 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetDescendants(includeSelf: true);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromTargetSiblings_InjectsFromFirstSibling()
        {
            const string scopePath = "Root 1";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1/Child 2/GrandChild 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromTargetSiblings();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }
    }
}