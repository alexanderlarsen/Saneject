using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.Special
{
    public class FromMethodTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent serviceInstance = root.AddComponent<InjectableComponent>();
            Requester requester = child.AddComponent<Requester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromMethod(() => serviceInstance);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent serviceInstance = root.AddComponent<InjectableComponent>();
            Requester requester = child.AddComponent<Requester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromMethod(() => serviceInstance);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            
            child.transform.SetParent(root.transform);
        }
    }
}