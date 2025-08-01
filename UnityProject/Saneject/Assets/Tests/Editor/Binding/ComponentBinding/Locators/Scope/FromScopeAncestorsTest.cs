using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Scope
{
    public class FromScopeAncestorsTest : BaseTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeAncestors();

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
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeAncestors();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
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