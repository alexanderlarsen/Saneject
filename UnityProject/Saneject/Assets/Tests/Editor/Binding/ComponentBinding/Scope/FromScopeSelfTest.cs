using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Scope
{
    public class FromScopeSelfTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            BasicRequester requester = root.AddComponent<BasicRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromScopeSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            BasicRequester requester = root.AddComponent<BasicRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromScopeSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}