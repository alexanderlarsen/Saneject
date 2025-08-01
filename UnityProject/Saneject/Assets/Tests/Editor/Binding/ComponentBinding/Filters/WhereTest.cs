using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereTest : BaseBindingTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void InjectsConcrete_WhenCustomPredicateMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            childA.AddComponent<InjectableComponent>();
            InjectableComponent componentB = childB.AddComponent<InjectableComponent>(); // index 1
            childC.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<InjectableComponent>()
                .FromRootDescendants()
                .Where(c => c.transform.GetSiblingIndex() % 2 == 1);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentB, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenCustomPredicateMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<TestScope>();
            Requester requester = root.AddComponent<Requester>();
            childA.AddComponent<InjectableComponent>();
            InjectableComponent componentB = childB.AddComponent<InjectableComponent>(); // index 1
            childC.AddComponent<InjectableComponent>();

            // Set up bindings
            root.GetComponent<TestScope>()
                .BindComponent<IInjectable, InjectableComponent>()
                .FromRootDescendants()
                .Where(c => c.transform.GetSiblingIndex() % 2 == 1);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentB, requester.interfaceComponent);
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