using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Filtering
{
    public class FilterByLastSiblingTest
    {
        private TestFilterComponent testFilterComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList().ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");

            // Create consumer first so it's not the last sibling
            GameObject consumer = new("Consumer");
            consumer.transform.SetParent(root.transform);
            testFilterComponent = consumer.AddComponent<TestFilterComponent>();

            GameObject go1 = new("FirstChild");
            go1.transform.SetParent(root.transform);
            go1.AddComponent<InjectableService>();

            GameObject go2 = new("SecondChild");
            go2.transform.SetParent(root.transform);
            go2.AddComponent<InjectableService>();

            GameObject go3 = new("LastChild");
            go3.transform.SetParent(root.transform);
            go3.AddComponent<InjectableService>();

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
        public void FiltersComponentByLastSibling()
        {
            Assert.AreEqual("LastChild", testFilterComponent.componentTarget.gameObject.name);
        }

        [Test]
        public void FiltersTransformByLastSibling()
        {
            Assert.AreEqual("LastChild", testFilterComponent.transformTarget.name);
        }

        public class TestScope : Scope
        {
            public override void Configure()
            {
                Bind<InjectableService>()
                    .FromScopeDescendants(includeSelf: false)
                    .WhereTransformIsLastSibling();

                Bind<Transform>()
                    .FromScopeDescendants(includeSelf: false)
                    .WhereTransformIsLastSibling();

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