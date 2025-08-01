using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereTagIsTest : BaseBindingTest
    {
        private GameObject root, taggedChild, untaggedChild;

        [Test]
        public void InjectsConcrete_WhenTagMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            InjectableComponent component = taggedChild.AddComponent<InjectableComponent>();
            untaggedChild.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<InjectableComponent>()
                .FromRootDescendants()
                .WhereTagIs("Test");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenTagMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            InjectableComponent component = taggedChild.AddComponent<InjectableComponent>();
            untaggedChild.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<IInjectable, InjectableComponent>()
                .FromRootDescendants()
                .WhereTagIs("Test");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            taggedChild = new GameObject();
            untaggedChild = new GameObject();
            taggedChild.tag = "Test";

            taggedChild.transform.SetParent(root.transform);
            untaggedChild.transform.SetParent(root.transform);
        }
    }
}