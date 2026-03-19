using NUnit.Framework;
using Plugins.SanejectLegacy.Editor.Core;
using Tests.SanejectLegacy.Runtime;
using Tests.SanejectLegacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.SanejectLegacy.Runtime.Component.ComponentRequester;

namespace Tests.SanejectLegacy.Editor.Bindings.ComponentBinding.Locators.Scope
{
    public class FromScopeParentTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeParent();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeParent();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();

            child.transform.SetParent(root.transform);
        }
    }
}