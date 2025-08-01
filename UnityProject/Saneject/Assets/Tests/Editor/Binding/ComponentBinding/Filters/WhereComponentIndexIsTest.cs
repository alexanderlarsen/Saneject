using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereComponentIndexIsTest : BaseBindingTest
    {
        private GameObject root, child;

        [Test]
        public void InjectsConcrete_ByComponentIndex()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>(); // index 0
            InjectableComponent targetComponent = root.AddComponent<InjectableComponent>(); // index 1
            TestScope scope = child.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .From(root.transform)
                .WhereComponentIndexIs(1);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.concreteComponent);
            Assert.AreEqual(targetComponent, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_ByComponentIndex()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            root.AddComponent<InjectableComponent>(); // index 0
            InjectableComponent targetComponent = root.AddComponent<InjectableComponent>(); // index 1
            TestScope scope = child.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .From(root.transform)
                .WhereComponentIndexIs(1);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
            Assert.AreEqual(targetComponent, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            child.transform.SetParent(root.transform);
        }
    }
}