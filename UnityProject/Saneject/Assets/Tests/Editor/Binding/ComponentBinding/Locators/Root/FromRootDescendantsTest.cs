using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Root
{
    public class FromRootDescendantsTest : BaseTest
    {
        private GameObject rootA, childA, grandchildA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();
            grandchildA.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = childA.AddComponent<TestScope>();
            ComponentRequester requester = childA.AddComponent<ComponentRequester>();
            grandchildA.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenRootHasNoDescendants()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = rootB.AddComponent<TestScope>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootDescendants();
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA = new GameObject();
            grandchildA = new GameObject();
            rootB = new GameObject();

            childA.transform.SetParent(rootA.transform);
            grandchildA.transform.SetParent(childA.transform);
        }
    }
}