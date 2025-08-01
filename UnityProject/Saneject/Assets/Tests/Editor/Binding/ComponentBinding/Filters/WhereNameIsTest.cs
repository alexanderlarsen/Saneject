using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereNameIsTest : BaseBindingTest
    {
        private GameObject root, childA, childB;

        [Test]
        public void InjectsComponentWithMatchingName_Concrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromAnywhereInScene()
                .WhereNameIs("InjectTarget");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void InjectsComponentWithMatchingName_Interface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent target = childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromAnywhereInScene()
                .WhereNameIs("InjectTarget");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(target, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            childA = new GameObject("InjectTarget");
            childB = new GameObject("Other");

            childA.transform.SetParent(root.transform);
            childB.transform.SetParent(root.transform);
        }
    }
}