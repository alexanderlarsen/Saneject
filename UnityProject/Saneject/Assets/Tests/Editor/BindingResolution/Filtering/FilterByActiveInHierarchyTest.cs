using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByActiveInHierarchyTest
    {
        private TestFilterComponent testFilterComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            GameObject go1 = new("Inactive");
            go1.SetActive(false);
            go1.transform.SetParent(root.transform);
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("Active");
            go2.SetActive(true);
            go2.transform.SetParent(root.transform);
            go2.AddComponent<InjectableService>();

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
        public void FiltersComponentByActiveInHierarchy()
        {
            Assert.AreEqual("Active", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByActiveInHierarchy()
        {
            
            Assert.AreEqual("Active", testFilterComponent.transformTarget.name);
        }

        public class TestScope : Scope
        {
            public override void Configure()
            {
                Bind<InjectableService>()
                    .FromScopeDescendants()
                    .WhereActiveInHierarchy();

                Bind<Transform>()
                    .FromScopeDescendants(includeSelf: false)
                    .WhereActiveInHierarchy();

                Bind<GameObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("Prefab 2");

                Bind<TestScriptableObject>()
                    .FromResourcesAll("Test")
                    .WhereNameIs("TestScriptableObject 2");
            }
        }
    }
}