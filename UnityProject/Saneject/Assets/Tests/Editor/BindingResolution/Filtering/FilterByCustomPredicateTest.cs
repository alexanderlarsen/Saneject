using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByCustomPredicateTest
    {
        private TestFilterComponent testFilterComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            // Create services with different GameObject names for filtering
            GameObject go1 = new("ServiceA");
            go1.transform.SetParent(root.transform);
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("ServiceLongName");
            go2.transform.SetParent(root.transform);
            go2.AddComponent<InjectableService>();

            GameObject go3 = new("ServiceC");
            go3.transform.SetParent(root.transform);
            go3.AddComponent<InjectableService>();

            // Create transforms at different positions for transform filtering
            GameObject t1 = new("TransformOne");
            t1.transform.SetParent(root.transform);
            t1.transform.position = new Vector3(1, 0, 0);

            GameObject t2 = new("TransformTwo");
            t2.transform.SetParent(root.transform);
            t2.transform.position = new Vector3(5, 0, 0);

            GameObject t3 = new("TransformThree");
            t3.transform.SetParent(root.transform);
            t3.transform.position = new Vector3(10, 0, 0);

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(root.transform);
            testFilterComponent = consumer.AddComponent<TestFilterComponent>();

            root.AddComponent<TestScope>();

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
        public void InjectsScriptableObject()
        {
            Assert.NotNull(testFilterComponent.scriptableObjectTarget);
        }

        [Test]
        public void FiltersComponentByCustomPredicate()
        {
            // Should inject service with GameObject name length > 10 characters
            Assert.AreEqual("ServiceLongName", testFilterComponent.componentTarget.gameObject.name);
            Assert.Greater(testFilterComponent.componentTarget.gameObject.name.Length, 10);
        }

        [Test]
        public void FiltersTransformByCustomPredicate()
        {
            // Should inject transform with x position > 3
            Assert.AreEqual("TransformTwo", testFilterComponent.transformTarget.name);
            Assert.Greater(testFilterComponent.transformTarget.position.x, 3f);
        }

        [Test]
        public void FiltersPrefabByCustomPredicate()
        {
            // Should inject prefab with name length > 7 characters
            Assert.NotNull(testFilterComponent.prefabTarget);
            Assert.Greater(testFilterComponent.prefabTarget.name.Length, 7);
        }

        [Test]
        public void FiltersScriptableObjectByCustomPredicate()
        {
            // Should inject scriptable object with name containing "2"
            Assert.NotNull(testFilterComponent.scriptableObjectTarget);
            Assert.That(testFilterComponent.scriptableObjectTarget.name, Does.Contain("2"));
        }

        public class TestScope : Scope
        {
            public override void Configure()
            {
                Bind<InjectableService>()
                    .FromScopeDescendants()
                    .Where(service => service.gameObject.name.Length > 10);

                Bind<Transform>()
                    .FromScopeDescendants(includeSelf: false)
                    .Where(transform => transform.position.x > 3f);

                Bind<GameObject>()
                    .FromResourcesAll("Test")
                    .Where(prefab => prefab.name.Length > 7);

                Bind<TestScriptableObject>()
                    .FromResourcesAll("Test")
                    .Where(so => so.name.Contains("2"));
            }
        }
    }
}