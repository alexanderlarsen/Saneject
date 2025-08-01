using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereIsLastComponentSiblingTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsLastComponentSibling_Concrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>(); // Should be ignored
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();
            InjectableComponent last = root.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>()
                .FromScopeSelf()
                .WhereIsLastComponentSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(last, requester.concreteComponent);
        }

        [Test]
        public void InjectsLastComponentSibling_Interface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>(); // Should be ignored
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();
            InjectableComponent last = root.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>()
                .FromScopeSelf()
                .WhereIsLastComponentSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(last, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}