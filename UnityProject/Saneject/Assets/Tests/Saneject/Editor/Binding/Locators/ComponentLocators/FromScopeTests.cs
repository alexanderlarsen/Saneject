using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Editor.Extensions;
using Tests.Saneject.Runtime.Fixtures;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tests.Saneject.Editor.Binding.Locators.ComponentLocators
{
    public class FromScopeTests
    {
        private Transform sceneRoot;

        [SetUp]
        public void SetUp()
        {
            sceneRoot = EditorSceneManager
                .OpenScene("Assets/Tests/Saneject/Runtime/Fixtures/LocatorScene.unity", OpenSceneMode.Single)
                .GetRootGameObjects()[0]
                .transform;
        }

        [Test]
        public void FromScopeSelf_InjectsFromSelf()
        {
            const string scopePath = "Root 1";
            const string requesterPath = "Root 1";
            const string dependencyPath = "Root 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromSelf();

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeParent_InjectsFromParent()
        {
            const string scopePath = "Root 1/Child 1";
            const string requesterPath = "Root 1/Child 1";
            const string dependencyPath = "Root 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromParent();

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestAncestor()
        {
            const string scopePath = "Root 1/Child 1/GrandChild 1";
            const string requesterPath = "Root 1/Child 1/GrandChild 1";
            const string dependencyPath = "Root 1/Child 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromAncestors(includeSelf: false);

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeAncestors_WHEN_IncludeSelfIsTrue_THEN_InjectsFromSelf()
        {
            const string scopePath = "Root 1/Child 1/GrandChild 1";
            const string requesterPath = "Root 1/Child 1/GrandChild 1";
            const string dependencyPath = "Root 1/Child 1/GrandChild 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromAncestors(includeSelf: true);

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeFirstChild_InjectsFromFirstChild()
        {
            const string scopePath = "Root 1";
            const string requesterPath = "Root 1";
            const string dependencyPath = "Root 1/Child 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromFirstChild();

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeLastChild_InjectsFromLastChild()
        {
            const string scopePath = "Root 1";
            const string requesterPath = "Root 1";
            const string dependencyPath = "Root 1/Child 3";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromLastChild();

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeChildWithIndex_InjectsFromChildWithIndex()
        {
            const string scopePath = "Root 1";
            const string requesterPath = "Root 1";
            const string dependencyPath = "Root 1/Child 2";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromChildWithIndex(1);

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsFalse_THEN_InjectsFromNearestDescendant()
        {
            const string scopePath = "Root 1/Child 1";
            const string requesterPath = "Root 1/Child 1";
            const string dependencyPath = "Root 1/Child 1/GrandChild 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromDescendants(includeSelf: false);

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeDescendants_WHEN_IncludeSelfIsTrue_THEN_InjectsFromNearestDescendant()
        {
            const string scopePath = "Root 1/Child 2";
            const string requesterPath = "Root 1/Child 2";
            const string dependencyPath = "Root 1/Child 2";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromDescendants(includeSelf: true);

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }

        [Test]
        public void FromScopeSiblings_InjectsFromFirstSibling()
        {
            const string scopePath = "Root 1/Child 2";
            const string requesterPath = "Root 1/Child 2";
            const string dependencyPath = "Root 1/Child 1";

            sceneRoot
                .AddComponentAtPath<TestScope>(scopePath)
                .BindComponent<Dependency>()
                .FromSiblings();

            ConcreteRequester requester = sceneRoot.AddComponentAtPath<ConcreteRequester>(requesterPath);
            Dependency dependency = sceneRoot.GetComponentAtPath<Dependency>(dependencyPath);
            InjectionRunner.Run(sceneRoot.ToArray(), ContextWalkFilter.SceneObjects);
            Assert.AreEqual(dependency, requester.dependency);
        }
    }
}