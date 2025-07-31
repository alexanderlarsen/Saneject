using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Special
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
            BasicRequester requester = rootA.AddComponent<BasicRequester>();
            rootB.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromAnywhereInScene();

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
            BasicRequester requester = rootA.AddComponent<BasicRequester>();
            rootB.AddComponent<InjectableComponent>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromAnywhereInScene();

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
            BasicRequester requester = rootA.AddComponent<BasicRequester>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromAnywhereInScene();
            scope.BindComponent<IInjectable, InjectableComponent>().FromAnywhereInScene();

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