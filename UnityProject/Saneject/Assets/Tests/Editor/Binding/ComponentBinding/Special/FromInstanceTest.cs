using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Special
{
    public class FromInstanceTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent instance = root.AddComponent<InjectableComponent>();
            BasicRequester requester = child.AddComponent<BasicRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromInstance(instance);

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
            InjectableComponent instance = root.AddComponent<InjectableComponent>();
            BasicRequester requester = child.AddComponent<BasicRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromInstance(instance);

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