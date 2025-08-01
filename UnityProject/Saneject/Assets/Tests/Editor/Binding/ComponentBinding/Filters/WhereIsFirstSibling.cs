using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereIsFirstSiblingTest : BaseBindingTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void InjectsConcrete_WhenFirstSibling()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            childC.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<InjectableComponent>()
                .FromRootDescendants()
                .WhereIsFirstSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenFirstSibling()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            childC.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<IInjectable, InjectableComponent>()
                .FromRootDescendants()
                .WhereIsFirstSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(target, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            childA = new GameObject();
            childB = new GameObject();
            childC = new GameObject();

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
            childC.transform.SetParent(root.transform);
        }
    }
}