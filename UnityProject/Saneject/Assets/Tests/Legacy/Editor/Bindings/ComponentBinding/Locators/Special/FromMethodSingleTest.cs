using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Locators.Special
{
    public class FromMethodSingleTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent serviceInstance = root.AddComponent<InjectableComponent>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromMethod(() => serviceInstance);

            // Inject
            DependencyInjector.InjectCurrentScene();

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
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            TestScope scope = child.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromMethod(() => serviceInstance);

            // Inject
            DependencyInjector.InjectCurrentScene();

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