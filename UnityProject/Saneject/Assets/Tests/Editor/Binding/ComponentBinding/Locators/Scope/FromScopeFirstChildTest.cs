using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Scope
{
    public class FromScopeFirstChildTest : BaseBindingTest
    {
        private GameObject root, child1, child2;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();
            
            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeFirstChild();

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
            Requester requester = root.AddComponent<Requester>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeFirstChild();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenFirstChildHasNoComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            child2.AddComponent<InjectableComponent>(); // Wrong child

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeFirstChild();
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeFirstChild();

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

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
        }
    }
}