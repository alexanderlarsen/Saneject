using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Target
{
    public class FromTargetLastChildTest : BaseBindingTest
    {
        private GameObject root, child, grandChild1, grandChild2;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            BasicRequester requester = child.AddComponent<BasicRequester>();
            grandChild2.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetLastChild();

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
            BasicRequester requester = child.AddComponent<BasicRequester>();
            grandChild2.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetLastChild();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenLastChildIsMissingComponent()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            BasicRequester requester = child.AddComponent<BasicRequester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetLastChild();
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetLastChild();

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
            grandChild1 = new GameObject();
            grandChild2 = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild1.transform.SetParent(child.transform);
            grandChild2.transform.SetParent(child.transform);
        }
    }
}