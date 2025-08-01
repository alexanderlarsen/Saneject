using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Special
{
    public class FromAnywhereInSceneTest : BaseBindingTest
    {
        private GameObject rootA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = rootA.AddComponent<TestScope>();
            ComponentRequester requester = rootA.AddComponent<ComponentRequester>();
            rootB.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAnywhereInScene();

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
            TestScope scope = rootA.AddComponent<TestScope>();
            ComponentRequester requester = rootA.AddComponent<ComponentRequester>();
            rootB.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromAnywhereInScene();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoMatchingComponentInScene()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = rootA.AddComponent<TestScope>();
            ComponentRequester requester = rootA.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAnywhereInScene();
            BindComponent<IInjectable, InjectableComponent>(scope).FromAnywhereInScene();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            rootB = new GameObject();
        }
    }
}