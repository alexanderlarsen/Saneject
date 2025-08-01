using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereIsFirstComponentSiblingTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsFirstComponentSibling_Concrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent first = root.AddComponent<InjectableComponent>();
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            root.AddComponent<InjectableComponent>(); // This one should not be injected

            // Set up bindings
            scope.BindComponent<InjectableComponent>()
                .FromScopeSelf()
                .WhereIsFirstComponentSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(first, requester.concreteComponent);
        }

        [Test]
        public void InjectsFirstComponentSibling_Interface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent first = root.AddComponent<InjectableComponent>();
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            root.AddComponent<InjectableComponent>(); // Should be ignored

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>()
                .FromScopeSelf()
                .WhereIsFirstComponentSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(first, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}