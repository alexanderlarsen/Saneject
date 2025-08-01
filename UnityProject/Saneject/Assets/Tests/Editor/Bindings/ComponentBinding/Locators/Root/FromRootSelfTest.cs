using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Root
{
    public class FromRootSelfTest : BaseBindingTest
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