using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
using Tests.Runtime;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.BindingResolution.Collection
{
    public class CollectionBindingsTest
    {
        private CollectionTestComponent testComponent;

        [SetUp]
        public void Setup()
        {
            LogAssert.ignoreFailingMessages = true;

            Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList()
                .ForEach(Object.DestroyImmediate);

            GameObject root = new("Root");
            GameObject mid = new("Mid");
            mid.transform.SetParent(root.transform);

            GameObject leaf1 = new("Leaf1");
            leaf1.transform.SetParent(mid.transform);
            leaf1.AddComponent<InjectableService>();

            GameObject leaf2 = new("Leaf2");
            leaf2.transform.SetParent(mid.transform);
            leaf2.AddComponent<InjectableService>();

            GameObject leaf3 = new("Leaf3");
            leaf3.transform.SetParent(mid.transform);
            leaf3.AddComponent<InjectableService>();

            GameObject leaf4 = new("Leaf4");
            leaf4.transform.SetParent(mid.transform);
            leaf4.AddComponent<InjectableService>();

            root.AddComponent<TestScope>();
            testComponent = mid.AddComponent<CollectionTestComponent>();

            DependencyInjector.InjectSceneDependencies();
        }

        [Test]
        public void ListsAndArraysAreNotNull()
        {
            Assert.NotNull(testComponent.servicesConcreteArray);
            Assert.NotNull(testComponent.servicesConcreteList);
            Assert.NotNull(testComponent.servicesInterfaceArray);
            Assert.NotNull(testComponent.servicesInterfaceList);
        }

        [Test]
        public void InterfaceAndConcreteCollectionsAreDistinct()
        {
            HashSet<string> concreteNames = testComponent.servicesConcreteArray
                .Select(c => c.gameObject.name).ToHashSet();

            HashSet<string> interfaceNames = testComponent.servicesInterfaceArray
                .Select(i => ((Component)i).gameObject.name).ToHashSet();

            CollectionAssert.AreNotEquivalent(concreteNames, interfaceNames);
            Assert.IsTrue(concreteNames.SetEquals(new[] { "Leaf1", "Leaf2" }));
            Assert.IsTrue(interfaceNames.SetEquals(new[] { "Leaf3", "Leaf4" }));
        }

        [Test]
        public void SingleFieldsRemainNull()
        {
            Assert.IsNull(testComponent.serviceConcreteSingle);
            Assert.IsNull(testComponent.serviceInterfaceSingle);
        }

        public class TestScope : Scope
        {
            public override void Configure()
            {
                Bind<InjectableService>()
                    .AsCollection()
                    .FromDescendants()
                    .Where(service => service.gameObject.name is "Leaf1" or "Leaf2");

                Bind<IInjectableService, InjectableService>()
                    .AsCollection()
                    .FromDescendants()
                    .Where(service => service.gameObject.name is "Leaf3" or "Leaf4");
            }
        }
    }
}