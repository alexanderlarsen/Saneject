using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Locators.Scope
{
    public class FromScopeChildWithIndexTest : BaseBindingTest
    {
        private GameObject root, child1, child2, child3;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeChildWithIndex(2);

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
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeChildWithIndex(2);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenInvalidIndex()
        {
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            child3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeChildWithIndex(999);
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeChildWithIndex(999);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();
            child3 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
            child3.transform.SetParent(root.transform);
        }
    }
}