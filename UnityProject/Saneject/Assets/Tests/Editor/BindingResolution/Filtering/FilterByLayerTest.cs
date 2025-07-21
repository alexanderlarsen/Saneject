using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByLayerTest
    {
        private TestFilterComponent testFilterComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            int testLayer = LayerMask.NameToLayer("Test");

            GameObject root = new("Root");

            GameObject go1 = new("One");
            go1.layer = 0;
            go1.transform.SetParent(root.transform);
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("Two");
            go2.layer = testLayer;
            go2.transform.SetParent(root.transform);
            go2.AddComponent<InjectableService>();

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(root.transform);
            testFilterComponent = consumer.AddComponent<TestFilterComponent>();

            TestScope scope = root.AddComponent<TestScope>();
            scope.filterLayer = testLayer;

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsComponent()
        {
            Assert.NotNull(testFilterComponent.componentTarget);
        }

        [Test]
        public void InjectsTransform()
        {
            Assert.NotNull(testFilterComponent.transformTarget);
        }

        [Test]
        public void InjectsPrefab()
        {
            Assert.NotNull(testFilterComponent.prefabTarget);
        }

        [Test]
        public void FiltersComponentByLayer()
        {
            Assert.AreEqual("Two", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByLayer()
        {
            Assert.AreEqual("Two", testFilterComponent.transformTarget.name);
        }

        [Test]
        public void FiltersPrefabByLayer()
        {
            Assert.AreEqual("Prefab 2", testFilterComponent.prefabTarget.name);
        }

        public class TestScope : Scope
        {
            public int filterLayer;

            public override void Configure()
            {
                Bind<InjectableService>()
                    .FromScopeDescendants()
                    .WhereLayerIs(filterLayer);

                Bind<Transform>()
                    .FromScopeDescendants()
                    .WhereLayerIs(filterLayer);

                Bind<GameObject>()
                    .FromResourcesAll("Test")
                    .WhereLayerIs(filterLayer);

                Bind<TestScriptableObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("TestScriptableObject 2");
            }
        }
    }
}