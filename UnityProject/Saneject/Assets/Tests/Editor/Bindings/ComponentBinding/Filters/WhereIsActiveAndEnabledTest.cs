using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class WhereIsActiveAndEnabledTest : BaseBindingTest
    {
        private GameObject root, activeChild, inactiveChild;

        [Test]
        public void InjectsConcrete_WhenActiveAndEnabled()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent enabledComponent = activeChild.AddComponent<InjectableComponent>();
            InjectableComponent disabledComponent = inactiveChild.AddComponent<InjectableComponent>();
            disabledComponent.enabled = false;

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsActiveAndEnabled();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
            Assert.AreEqual(enabledComponent, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenActiveAndEnabled()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent enabledComponent = activeChild.AddComponent<InjectableComponent>();
            InjectableComponent disabledComponent = inactiveChild.AddComponent<InjectableComponent>();
            disabledComponent.enabled = false;

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsActiveAndEnabled();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
            Assert.AreEqual(enabledComponent, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            activeChild = new GameObject();
            inactiveChild = new GameObject();

            activeChild.transform.SetParent(root.transform);
            inactiveChild.transform.SetParent(root.transform);

            inactiveChild.SetActive(false);
        }
    }
}