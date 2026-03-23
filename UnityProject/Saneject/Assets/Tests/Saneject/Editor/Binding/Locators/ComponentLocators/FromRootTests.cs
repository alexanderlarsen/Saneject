using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Editor.Extensions;
using Tests.Saneject.Runtime.Fixtures;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromRootTests
    {
        private const string LocatorScenePath = "Assets/Tests/Saneject/Runtime/Fixtures/LocatorScene.unity";

        private Scene scene;

        [SetUp]
        public void SetUp()
        {
            scene = EditorSceneManager.OpenScene(LocatorScenePath, OpenSceneMode.Single);
        }

        [Test]
        public void FromRootSelf_InjectsFromSelf()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootSelf();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromRootFirstChild_InjectsFromFirstChild()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1/Child 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootFirstChild();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromRootLastChild_InjectsFromLastChild()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1/Child 3";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootLastChild();

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromRootChildWithIndex_InjectsFromChildWithIndex()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1/Child 2";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootChildWithIndex(1);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromRootDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1/Child 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootDescendants(includeSelf: false);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }

        [Test]
        public void FromRootDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            const string scopePath = "Root 1/Child 2/GrandChild 3";
            const string targetPath = "Root 1/Child 2/GrandChild 3";
            const string expectedDependencyPath = "Root 1";

            scene.AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromRootDescendants(includeSelf: true);

            ConcreteTarget target = scene.AddComponentAtPath<ConcreteTarget>(targetPath);
            Dependency expectedDependency = scene.GetComponentAtPath<Dependency>(expectedDependencyPath);
            InjectionRunner.Run(scene.GetRootGameObjects(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(expectedDependency, target.dependency);
        }
    }
}