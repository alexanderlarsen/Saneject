using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Legacy;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Collection
{
    public class ComponentCollectionBindingTest : BaseBindingTest
    {
        private GameObject root, child, grandChild1, grandChild2, grandChild3, grandChild4;

        [Test]
        public void ListsAndArraysAreNotNull()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentCollectionRequester requester = child.AddComponent<ComponentCollectionRequester>();
            grandChild1.AddComponent<InjectableComponent>();
            grandChild2.AddComponent<InjectableComponent>();
            grandChild3.AddComponent<InjectableComponent>();
            grandChild4.AddComponent<InjectableComponent>();

            // Set up bindings
            BindMultipleComponents<InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild1 || service.gameObject == grandChild2);

            BindMultipleComponents<IInjectable, InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild3 || service.gameObject == grandChild4);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.servicesConcreteArray);
            Assert.NotNull(requester.servicesConcreteList);
            Assert.NotNull(requester.servicesInterfaceArray);
            Assert.NotNull(requester.servicesInterfaceList);
        }

        [Test]
        public void InterfaceAndConcreteCollectionsAreDistinct()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentCollectionRequester requester = child.AddComponent<ComponentCollectionRequester>();
            grandChild1.AddComponent<InjectableComponent>();
            grandChild2.AddComponent<InjectableComponent>();
            grandChild3.AddComponent<InjectableComponent>();
            grandChild4.AddComponent<InjectableComponent>();

            // Set up bindings
            BindMultipleComponents<InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild1 || service.gameObject == grandChild2);

            BindMultipleComponents<IInjectable, InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild3 || service.gameObject == grandChild4);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            HashSet<GameObject> concreteSet = requester.servicesConcreteArray.Select(c => c.gameObject).ToHashSet();
            HashSet<GameObject> interfaceSet = requester.servicesInterfaceArray.Select(i => ((Component)i).gameObject).ToHashSet();

            Assert.IsTrue(concreteSet.SetEquals(new[] { grandChild1, grandChild2 }));
            Assert.IsTrue(interfaceSet.SetEquals(new[] { grandChild3, grandChild4 }));
            CollectionAssert.AreNotEquivalent(concreteSet, interfaceSet);
        }

        [Test]
        public void SingleFieldsRemainNull()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentCollectionRequester requester = child.AddComponent<ComponentCollectionRequester>();
            grandChild1.AddComponent<InjectableComponent>();
            grandChild2.AddComponent<InjectableComponent>();
            grandChild3.AddComponent<InjectableComponent>();
            grandChild4.AddComponent<InjectableComponent>();

            // Set up bindings
            BindMultipleComponents<InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild1 || service.gameObject == grandChild2);

            BindMultipleComponents<IInjectable, InjectableComponent>(scope)
                .FromDescendants()
                .Where(service => service.gameObject == grandChild3 || service.gameObject == grandChild4);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.serviceConcreteSingle);
            Assert.IsNull(requester.serviceInterfaceSingle);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            grandChild1 = new GameObject();
            grandChild2 = new GameObject();
            grandChild3 = new GameObject();
            grandChild4 = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild1.transform.SetParent(child.transform);
            grandChild2.transform.SetParent(child.transform);
            grandChild3.transform.SetParent(child.transform);
            grandChild4.transform.SetParent(child.transform);
        }
    }
}