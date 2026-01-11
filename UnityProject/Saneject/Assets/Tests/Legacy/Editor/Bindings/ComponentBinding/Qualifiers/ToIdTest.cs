using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequesterWithID = Tests.Legacy.Runtime.Component.ComponentRequesterWithID;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Qualifiers
{
    public class ToIdTest : BaseBindingTest
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
            BindComponent<InjectableComponent>(scope).ToID("componentA").FromInstance(componentA);
            BindComponent<InjectableComponent>(scope).ToID("componentB").FromInstance(componentB);

            // Inject
            DependencyInjector.InjectCurrentScene();

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
            BindComponent<IInjectable, InjectableComponent>(scope).ToID("componentA").FromInstance(componentA);
            BindComponent<IInjectable, InjectableComponent>(scope).ToID("componentB").FromInstance(componentB);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.interfaceComponentA);
            Assert.NotNull(requester.interfaceComponentB);
            Assert.AreEqual(requester.interfaceComponentA, componentA);
            Assert.AreEqual(requester.interfaceComponentB, componentB);
            Assert.AreNotEqual(requester.interfaceComponentA, requester.interfaceComponentB);
        }
        
        [Test]
        public void NonIdBindingDoesNotInjectIntoIdField()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequesterWithID requester = root.AddComponent<ComponentRequesterWithID>();
            InjectableComponent component = child1.AddComponent<InjectableComponent>();

            // Set up a binding WITHOUT ID
            BindComponent<InjectableComponent>(scope).FromInstance(component);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert – ID fields should remain null because no matching ID binding exists
            Assert.IsNull(requester.concreteComponentA);
            Assert.IsNull(requester.concreteComponentB);
            Assert.IsNull(requester.interfaceComponentA);
            Assert.IsNull(requester.interfaceComponentB);
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