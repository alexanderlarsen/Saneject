using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Scope
{
    public class FromScopeSiblingsTest : BaseTest
    {
        private GameObject parent, child1, child2;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            ComponentRequester requester = child2.AddComponent<ComponentRequester>();
            TestScope scope = child2.AddComponent<TestScope>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeSiblings();

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
            ComponentRequester requester = child2.AddComponent<ComponentRequester>();
            TestScope scope = child2.AddComponent<TestScope>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeSiblings();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoSiblingHasComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            ComponentRequester requester = child2.AddComponent<ComponentRequester>();
            TestScope scope = child2.AddComponent<TestScope>();
            child2.AddComponent<InjectableComponent>(); // same object, not sibling

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeSiblings();
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeSiblings();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            parent = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();

            child1.transform.SetParent(parent.transform);
            child2.transform.SetParent(parent.transform);
        }
    }
}