using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereActiveSelfTest : BaseTest
    {
        private GameObject root, activeChild, inactiveChild;

        [Test]
        public void InjectsConcrete_WhenActiveSelf()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent inactiveComponent = inactiveChild.AddComponent<InjectableComponent>();
            InjectableComponent activeComponent = activeChild.AddComponent<InjectableComponent>();
            inactiveChild.SetActive(false);

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereActiveSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(activeComponent, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenActiveSelf()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent inactiveComponent = inactiveChild.AddComponent<InjectableComponent>();
            InjectableComponent activeComponent = activeChild.AddComponent<InjectableComponent>();
            inactiveChild.SetActive(false);

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereActiveSelf();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(activeComponent, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            activeChild = new GameObject();
            inactiveChild = new GameObject();

            activeChild.transform.SetParent(root.transform);
            inactiveChild.transform.SetParent(root.transform);
        }
    }
}