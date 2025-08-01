using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereTargetIsTest : BaseBindingTest
    {
        private GameObject root, child1, child2, child3, child4, child5, child6;

        [Test]
        public void InjectsComponentOnlyIntoMatchingTarget()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requesterA = child3.AddComponent<ComponentRequester>();
            ComponentRequesterB requesterB = child4.AddComponent<ComponentRequesterB>();
            InjectableComponent componentA = child1.AddComponent<InjectableComponent>();
            InjectableComponent componentB = child2.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromInstance(componentA)
                .WhereTargetIs<ComponentRequester>();

            BindComponent<InjectableComponent>(scope)
                .FromInstance(componentB)
                .WhereTargetIs<ComponentRequesterB>();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentA, requesterA.concreteComponent);
            Assert.AreEqual(componentB, requesterB.concreteComponent);
        }

        [Test]
        public void InjectsInterfaceOnlyIntoMatchingTarget()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requesterA = child5.AddComponent<ComponentRequester>();
            ComponentRequesterB requesterB = child6.AddComponent<ComponentRequesterB>();
            InjectableComponent componentA = child1.AddComponent<InjectableComponent>();
            InjectableComponent componentB = child2.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromInstance(componentA)
                .WhereTargetIs<ComponentRequester>();

            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromInstance(componentB)
                .WhereTargetIs<ComponentRequesterB>();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(componentA, requesterA.interfaceComponent);
            Assert.AreEqual(componentB, requesterB.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();
            child3 = new GameObject();
            child4 = new GameObject();
            child5 = new GameObject();
            child6 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
            child3.transform.SetParent(root.transform);
            child4.transform.SetParent(root.transform);
            child5.transform.SetParent(root.transform);
            child6.transform.SetParent(root.transform);
        }
    }
}