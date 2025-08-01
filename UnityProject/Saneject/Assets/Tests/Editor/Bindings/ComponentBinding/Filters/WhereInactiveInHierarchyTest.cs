using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class WhereInactiveInHierarchyTest : BaseBindingTest
    {
        private GameObject root, activeChild, inactiveChild;

        [Test]
        public void InjectsConcrete_WhenInactiveInHierarchy()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            var scope =root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            activeChild.AddComponent<InjectableComponent>();
            InjectableComponent component = inactiveChild.AddComponent<InjectableComponent>();
            inactiveChild.SetActive(false);

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereInactiveInHierarchy();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenInactiveInHierarchy()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            var scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            activeChild.AddComponent<InjectableComponent>();
            InjectableComponent component = inactiveChild.AddComponent<InjectableComponent>();
            inactiveChild.SetActive(false);

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereInactiveInHierarchy();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.interfaceComponent);
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