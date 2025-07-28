using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByTagTest
    {
        private TestFilterComponent testFilterComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject go1 = new("One");
            go1.transform.SetParent(root.transform);
            go1.tag = "Untagged";
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("Two");
            go2.transform.SetParent(root.transform);
            go2.tag = "Player";
            go2.AddComponent<InjectableService>();

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(root.transform);
            testFilterComponent = consumer.AddComponent<TestFilterComponent>();

            TestScope scope = root.AddComponent<TestScope>();
            scope.filterTag = "Player";

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsComponent()
        {
            Assert.NotNull(testFilterComponent.componentTarget);
        }

        [Test]
        public void InjectTransform()
        {
            Assert.NotNull(testFilterComponent.transformTarget);
        }

        [Test]
        public void InjectsPrefab()
        {
            Assert.NotNull(testFilterComponent.prefabTarget);
        }

        [Test]
        public void FiltersComponentByTag()
        {
            Assert.AreEqual("Two", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByTag()
        {
            Assert.AreEqual("Two", testFilterComponent.transformTarget.name);
        }

        [Test]
        public void FiltersPrefabByTag()
        {
            Assert.AreEqual("Prefab 2", testFilterComponent.prefabTarget.name);
        }

        public class TestScope : Scope
        {
            public string filterTag;

            public override void Configure()
            {
                Bind<InjectableService>()
                    .FromScopeDescendants()
                    .WhereGameObjectTagIs(filterTag);

                Bind<Transform>()
                    .FromScopeDescendants()
                    .WhereGameObjectTagIs(filterTag);

                Bind<GameObject>()
                    .FromResourcesAll("Test")
                    .WhereGameObjectTagIs("Test");

                Bind<TestScriptableObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("TestScriptableObject 2");
            }
        }
    }
}