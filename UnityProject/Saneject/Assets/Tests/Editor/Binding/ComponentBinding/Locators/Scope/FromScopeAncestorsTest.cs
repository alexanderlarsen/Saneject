using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Scope
{
    public class FromScopeAncestorsTest : BaseBindingTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            Requester requester = grandChild.AddComponent<Requester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeAncestors();

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
            Requester requester = grandChild.AddComponent<Requester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeAncestors();

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