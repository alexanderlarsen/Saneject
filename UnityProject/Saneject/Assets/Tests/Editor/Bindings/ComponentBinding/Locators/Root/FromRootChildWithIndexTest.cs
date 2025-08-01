using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Root
{
    public class FromRootChildWithIndexTest : BaseBindingTest
    {
        private GameObject root, child1, child2, child3;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = child1.AddComponent<TestScope>();
            ComponentRequester requester = child1.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootChildWithIndex(2);

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
            TestScope scope = child1.AddComponent<TestScope>();
            ComponentRequester requester = child1.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootChildWithIndex(2);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenChildIndexIsInvalid()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = child1.AddComponent<TestScope>();
            ComponentRequester requester = child1.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromRootChildWithIndex(999);
            BindComponent<IInjectable, InjectableComponent>(scope).FromRootChildWithIndex(999);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();
            child3 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
            child3.transform.SetParent(root.transform);
        }
    }
}