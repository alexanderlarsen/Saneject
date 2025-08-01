using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequesterWithID = Tests.Runtime.Component.ComponentRequesterWithID;

namespace Tests.Editor.Bindings.ComponentBinding.Config
{
    public class WithIdTest : BaseBindingTest
    {
        private GameObject root, child1, child2;

        [Test]
        public void InjectsConcreteByID()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequesterWithID requester = root.AddComponent<ComponentRequesterWithID>();
            InjectableComponent componentA = child1.AddComponent<InjectableComponent>();
            InjectableComponent componentB = child2.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).WithId("componentA").FromInstance(componentA);
            BindComponent<InjectableComponent>(scope).WithId("componentB").FromInstance(componentB);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.concreteComponentA);
            Assert.NotNull(requester.concreteComponentB);
            Assert.AreEqual(requester.concreteComponentA, componentA);
            Assert.AreEqual(requester.concreteComponentB, componentB);
            Assert.AreNotEqual(requester.concreteComponentA, requester.concreteComponentB);
        }

        [Test]
        public void InjectsInterfaceByID()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequesterWithID requester = root.AddComponent<ComponentRequesterWithID>();
            InjectableComponent componentA = child1.AddComponent<InjectableComponent>();
            InjectableComponent componentB = child2.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).WithId("componentA").FromInstance(componentA);
            BindComponent<IInjectable, InjectableComponent>(scope).WithId("componentB").FromInstance(componentB);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponentA);
            Assert.NotNull(requester.interfaceComponentB);
            Assert.AreEqual(requester.interfaceComponentA, componentA);
            Assert.AreEqual(requester.interfaceComponentB, componentB);
            Assert.AreNotEqual(requester.interfaceComponentA, requester.interfaceComponentB);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
        }
    }
}