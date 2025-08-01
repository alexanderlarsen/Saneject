using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Scope
{
    public class FromScopeDescendantsTest : BaseBindingTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = child.AddComponent<Requester>();
            grandChild.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeDescendants();

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
            Requester requester = child.AddComponent<Requester>();
            grandChild.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoDescendantMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = child.AddComponent<Requester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeDescendants();
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeDescendants();

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