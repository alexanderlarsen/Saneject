using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Root
{
    public class FromRootSelfTest : BaseTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = child.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootSelf();

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
            TestScope scope = child.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenRootHasNoComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = child.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootSelf();
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            child.transform.SetParent(root.transform);
        }
    }
}