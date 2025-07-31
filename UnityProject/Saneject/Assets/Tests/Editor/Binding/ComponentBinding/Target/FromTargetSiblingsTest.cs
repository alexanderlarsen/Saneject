using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Target
{
    public class FromTargetSiblingsTest : BaseBindingTest
    {
        private GameObject parent, child1, child2, child3;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = parent.AddComponent<TestScope>();
            BasicRequester requester = child3.AddComponent<BasicRequester>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetSiblings();

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
            TestScope scope = parent.AddComponent<TestScope>();
            BasicRequester requester = child3.AddComponent<BasicRequester>();
            child1.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetSiblings();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoSiblingsHaveComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = parent.AddComponent<TestScope>();
            BasicRequester requester = child3.AddComponent<BasicRequester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetSiblings();
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetSiblings();

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
            child3 = new GameObject();

            child1.transform.SetParent(parent.transform);
            child2.transform.SetParent(parent.transform);
            child3.transform.SetParent(parent.transform);
        }
    }
}