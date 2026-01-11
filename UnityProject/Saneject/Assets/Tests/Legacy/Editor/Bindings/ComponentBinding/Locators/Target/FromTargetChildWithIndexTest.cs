using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Locators.Target
{
    public class FromTargetChildWithIndexTest : BaseBindingTest
    {
        private GameObject root, child, grandChild1, grandChild2, grandChild3;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();
            
            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            grandChild3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetChildWithIndex(2);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();
            
            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            grandChild3.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetChildWithIndex(2);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNotNull(requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenChildIndexIsInvalid()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromTargetChildWithIndex(99);
            BindComponent<IInjectable, InjectableComponent>(scope).FromTargetChildWithIndex(99);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            grandChild1 = new GameObject();
            grandChild2 = new GameObject();
            grandChild3 = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild1.transform.SetParent(child.transform);
            grandChild2.transform.SetParent(child.transform);
            grandChild3.transform.SetParent(child.transform);
        }
    }
}