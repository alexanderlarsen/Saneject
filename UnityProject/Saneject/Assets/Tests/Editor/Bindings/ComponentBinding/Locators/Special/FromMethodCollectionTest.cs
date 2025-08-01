using System.Collections.Generic;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Special
{
    public class FromMethodCollectionTest : BaseBindingTest
    {
        private GameObject root, childA, childB;

        [Test]
        public void InjectsConcrete_FromMethodCollection_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent componentA = childA.AddComponent<InjectableComponent>();
            componentA.name = "A";

            InjectableComponent componentB = childB.AddComponent<InjectableComponent>();
            componentB.name = "B";

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromMethod(() => new[] { componentA, componentB })
                .WhereNameIs("B");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentB, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_FromMethodCollection_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();

            InjectableComponent componentA = childA.AddComponent<InjectableComponent>();
            componentA.name = "A";

            InjectableComponent componentB = childB.AddComponent<InjectableComponent>();
            componentB.name = "B";

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromMethod(() => new List<InjectableComponent> { componentA, componentB })
                .WhereNameIs("B");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentB, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            childA = new GameObject();
            childB = new GameObject();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
        }
    }
}