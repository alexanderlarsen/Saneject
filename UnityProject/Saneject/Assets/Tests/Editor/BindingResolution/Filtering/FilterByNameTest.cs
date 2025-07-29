using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByNameTest
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
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("Two");
            go2.transform.SetParent(root.transform);
            go2.AddComponent<InjectableService>();

            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(root.transform);
            testFilterComponent = consumer.AddComponent<TestFilterComponent>();

            TestScope scope = root.AddComponent<TestScope>();
            scope.filterName = "Two";

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
        public void InjectsScriptableObjectFromResources()
        {
            Assert.NotNull(testFilterComponent.scriptableObjectTarget);
        }

        [Test]
        public void InjectsPrefabFromResources()
        {
            Assert.NotNull(testFilterComponent.prefabTarget);
        }

        [Test]
        public void FiltersComponentByName()
        {
            Assert.AreEqual("Two", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByName()
        {
            Assert.AreEqual("Two", testFilterComponent.transformTarget.name);
        }

        [Test]
        public void FiltersScriptableObjectByName()
        {
            Assert.AreEqual("TestScriptableObject 2", testFilterComponent.scriptableObjectTarget.name);
        }

        [Test]
        public void FiltersPrefabByName()
        {
            Assert.AreEqual("Prefab 2", testFilterComponent.prefabTarget.name);
        }

        public class TestScope : Scope
        {
            public string filterName;

            protected override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromScopeDescendants()
                    .WhereNameIs(filterName);

                BindComponent<Transform>()
                    .FromScopeDescendants()
                    .WhereNameIs(filterName);

                BindAsset<GameObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("Prefab 2");

                BindAsset<TestScriptableObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("TestScriptableObject 2");
            }
        }
    }
}