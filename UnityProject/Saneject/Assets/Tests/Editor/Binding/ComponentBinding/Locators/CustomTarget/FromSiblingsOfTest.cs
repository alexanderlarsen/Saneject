using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Locators.CustomTarget
{
    public class FromSiblingsOfTest : BaseBindingTest
    {
        private GameObject rootA, childA1, childA2, childA3;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            childA2.AddComponent<InjectableComponent>();
            Requester requester = rootB.AddComponent<Requester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromSiblingsOf(childA3.transform);

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
            childA2.AddComponent<InjectableComponent>();
            Requester requester = rootB.AddComponent<Requester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromSiblingsOf(childA3.transform);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA1 = new GameObject();
            childA2 = new GameObject();
            childA3 = new GameObject();
            rootB = new GameObject();

            childA1.transform.SetParent(rootA.transform);
            childA2.transform.SetParent(rootA.transform);
            childA3.transform.SetParent(rootA.transform);
        }
    }
}