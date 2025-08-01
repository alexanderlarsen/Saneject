using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Target
{
    public class FromTargetSelfTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = child.AddComponent<Requester>();
            child.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetSelf();

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
            child.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenComponentMissing()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            Requester requester = child.AddComponent<Requester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromTargetSelf();
            scope.BindComponent<IInjectable, InjectableComponent>().FromTargetSelf();

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
            child.transform.SetParent(root.transform);
        }
    }
}