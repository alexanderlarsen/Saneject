using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class WhereIsEnabledTest : BaseBindingTest
    {
        private GameObject root, enabledChild, disabledChild;

        [Test]
        public void InjectsConcrete_WhenEnabled()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent enabledComponent = enabledChild.AddComponent<InjectableComponent>();
            InjectableComponent disabledComponent = disabledChild.AddComponent<InjectableComponent>();
            disabledComponent.enabled = false;

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsEnabled();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
            Assert.AreEqual(enabledComponent, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenEnabled()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent enabledComponent = enabledChild.AddComponent<InjectableComponent>();
            InjectableComponent disabledComponent = disabledChild.AddComponent<InjectableComponent>();
            disabledComponent.enabled = false;

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsEnabled();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
            Assert.AreEqual(enabledComponent, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            enabledChild = new GameObject();
            disabledChild = new GameObject();

            enabledChild.transform.SetParent(root.transform);
            disabledChild.transform.SetParent(root.transform);
        }
    }
}