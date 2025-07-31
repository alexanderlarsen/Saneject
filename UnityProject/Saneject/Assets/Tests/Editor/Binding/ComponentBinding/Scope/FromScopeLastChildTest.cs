using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Scope
{
    public class FromScopeLastChildTest : BaseBindingTest
    {
        private GameObject root, child1, child2;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();
            
            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            BasicRequester requester = root.AddComponent<BasicRequester>();
            child2.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeLastChild();

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
            BasicRequester requester = root.AddComponent<BasicRequester>();
            child2.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeLastChild();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenLastChildHasNoComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            BasicRequester requester = root.AddComponent<BasicRequester>();
            child1.AddComponent<InjectableComponent>(); // Wrong child

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeLastChild();
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeLastChild();

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