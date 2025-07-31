using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Root
{
    public class FromRootDescendantsTest : BaseBindingTest
    {
        private GameObject rootA, childA, grandchildA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = childA.AddComponent<TestScope>();
            BasicRequester requester = childA.AddComponent<BasicRequester>();
            grandchildA.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromRootDescendants();

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
            TestScope scope = childA.AddComponent<TestScope>();
            BasicRequester requester = childA.AddComponent<BasicRequester>();
            grandchildA.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromRootDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenRootHasNoDescendants()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = rootB.AddComponent<TestScope>();
            BasicRequester requester = rootB.AddComponent<BasicRequester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromRootDescendants();
            scope.BindComponent<IInjectable, InjectableComponent>().FromRootDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA = new GameObject();
            grandchildA = new GameObject();
            rootB = new GameObject();

            childA.transform.SetParent(rootA.transform);
            grandchildA.transform.SetParent(childA.transform);
        }
    }
}