using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.CustomTarget
{
    public class FromAncestorsOfTest : BaseBindingTest
    {
        private GameObject rootA, childA, grandchildA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            rootA.AddComponent<InjectableComponent>();
            BasicRequester requester = rootB.AddComponent<BasicRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromAncestorsOf(grandchildA.transform);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            rootA.AddComponent<InjectableComponent>();
            BasicRequester requester = rootB.AddComponent<BasicRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromAncestorsOf(grandchildA.transform);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA = new GameObject();
            grandchildA = new GameObject();
            rootB = new GameObject();

            childA.transform.SetParent(rootA.transform);
            grandchildA.transform.SetParent(childA.transform);
        }
    }
}