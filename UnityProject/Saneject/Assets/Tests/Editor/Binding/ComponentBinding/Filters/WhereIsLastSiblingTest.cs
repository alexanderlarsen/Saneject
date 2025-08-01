using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereIsLastSiblingTest : BaseTest
    {
        private GameObject root, childA, childB, childC;

        [Test]
        public void InjectsConcrete_WhenLastSibling()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            InjectableComponent target = childC.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsLastSibling();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenLastSibling()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            childA.AddComponent<InjectableComponent>();
            childB.AddComponent<InjectableComponent>();
            InjectableComponent target = childC.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereIsLastSibling();

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