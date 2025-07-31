using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime.Legacy;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.Legacy.Filtering
{
    public class FilterByInactiveInHierarchyTest
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
        public void FiltersComponentByInactiveInHierarchy()
        {
            Assert.AreEqual("Inactive", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByInactiveInHierarchy()
        {
            Assert.AreEqual("Inactive", testFilterComponent.transformTarget.name);
        }

        public class TestScope : Scope
        {
            public override void ConfigureBindings()
            {
                BindComponent<InjectableService>()
                    .FromScopeDescendants()
                    .WhereInactiveInHierarchy();

                BindComponent<Transform>()
                    .FromScopeDescendants(includeSelf: false)
                    .WhereInactiveInHierarchy();

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