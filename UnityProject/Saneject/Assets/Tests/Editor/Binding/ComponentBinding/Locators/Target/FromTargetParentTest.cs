using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Target
{
    public class FromTargetParentTest : BaseTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            child.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetParent();

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
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            child.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetParent();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenParentMissingComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetParent();
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetParent();

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
            grandChild = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild.transform.SetParent(child.transform);
        }
    }
}