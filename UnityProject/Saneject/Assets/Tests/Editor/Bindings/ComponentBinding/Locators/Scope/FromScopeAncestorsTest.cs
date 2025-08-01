using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Scope
{
    public class FromScopeAncestorsTest : BaseBindingTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeAncestors();

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
            root.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromScopeAncestors();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void InjectsConcrete_IncludeSelfTrue()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent injectable = grandChild.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeAncestors(includeSelf: true);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(injectable, requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenOnlySelfPresentAndIncludeSelfFalse()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            grandChild.AddComponent<InjectableComponent>();
            ComponentRequester requester = grandChild.AddComponent<ComponentRequester>();
            TestScope scope = grandChild.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeAncestors(includeSelf: false);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoAncestors()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Create a disconnected object
            GameObject lone = new();
            ComponentRequester requester = lone.AddComponent<ComponentRequester>();
            TestScope scope = lone.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromScopeAncestors(includeSelf: false);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            grandChild = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild.transform.SetParent(child.transform);
        }
    }
}