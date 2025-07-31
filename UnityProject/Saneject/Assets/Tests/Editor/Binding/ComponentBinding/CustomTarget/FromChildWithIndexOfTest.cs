using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.CustomTarget
{
    public class FromChildWithIndexOfTest : BaseBindingTest
    {
        private GameObject rootA, childA, grandChildA1, grandChildA2, grandChildA3;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            grandChildA3.AddComponent<InjectableComponent>();
            BasicRequester requester = rootB.AddComponent<BasicRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<InjectableComponent>().FromChildWithIndexOf(childA.transform, 2);

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
            childA.transform.GetChild(2).gameObject.AddComponent<InjectableComponent>();
            BasicRequester requester = rootB.AddComponent<BasicRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            scope.BindComponent<IInjectable, InjectableComponent>().FromChildWithIndexOf(childA.transform, 2);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA = new GameObject();
            grandChildA1 = new GameObject();
            grandChildA2 = new GameObject();
            grandChildA3 = new GameObject();
            rootB = new GameObject();

            childA.transform.SetParent(rootA.transform);
            grandChildA1.transform.SetParent(childA.transform);
            grandChildA2.transform.SetParent(childA.transform);
            grandChildA3.transform.SetParent(childA.transform);
        }
    }
}