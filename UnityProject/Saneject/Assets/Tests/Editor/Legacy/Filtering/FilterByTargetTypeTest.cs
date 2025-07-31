using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime.Legacy;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.Legacy.Filtering
{
    public class FilterByTargetTypeTest
    {
        private TestFilterComponent testFilterComponent;
        private AnotherTestComponent anotherTestComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            // Create two services with different names
            GameObject serviceA = new("ServiceA");
            serviceA.transform.SetParent(root.transform);
            serviceA.AddComponent<InjectableService>();

            GameObject serviceB = new("ServiceB");
            serviceB.transform.SetParent(root.transform);
            serviceB.AddComponent<InjectableService>();

            // Create consumer components
            GameObject consumer1 = new("Consumer1");
            consumer1.transform.SetParent(root.transform);
            testFilterComponent = consumer1.AddComponent<TestFilterComponent>();

            GameObject consumer2 = new("Consumer2");
            consumer2.transform.SetParent(root.transform);
            anotherTestComponent = consumer2.AddComponent<AnotherTestComponent>();

            root.AddComponent<TestScope>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void InjectsComponentForTestFilterComponent()
        {
            Assert.NotNull(testFilterComponent.componentTarget);
        }

        [Test]
        public void InjectsComponentForAnotherTestComponent()
        {
            Assert.NotNull(anotherTestComponent.componentTarget);
        }

        [Test]
        public void InjectsOtherResourcesForTestFilterComponent()
        {
            Assert.NotNull(testFilterComponent.transformTarget);
            Assert.NotNull(testFilterComponent.prefabTarget);
            Assert.NotNull(testFilterComponent.scriptableObjectTarget);
        }

        [Test]
        public void FiltersServiceAByTargetType()
        {
            // TestFilterComponent should get ServiceA due to target filtering
            Assert.AreEqual("ServiceA", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersServiceBByTargetType()
        {
            // AnotherTestComponent should get ServiceB due to target filtering
            Assert.AreEqual("ServiceB", anotherTestComponent.componentTarget.gameObject.name);
        }

        public class TestScope : Scope
        {
            public override void ConfigureBindings()
            {
                // ServiceA only for TestFilterComponent
                BindComponent<InjectableService>()
                    .FromScopeDescendants()
                    .WhereNameIs("ServiceA")
                    .WhereTargetIs<TestFilterComponent>();

                // ServiceB only for AnotherTestComponent  
                BindComponent<InjectableService>()
                    .FromScopeDescendants()
                    .WhereNameIs("ServiceB")
                    .WhereTargetIs<AnotherTestComponent>();

                // Default bindings for other resources
                BindComponent<Transform>()
                    .FromScopeDescendants(includeSelf: false);

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