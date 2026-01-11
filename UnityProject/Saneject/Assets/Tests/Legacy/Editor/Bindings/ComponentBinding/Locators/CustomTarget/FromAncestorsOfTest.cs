using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Locators.CustomTarget
{
    public class FromAncestorsOfTest : BaseBindingTest
    {
        private GameObject rootA, childA, grandchildA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            rootA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAncestorsOf(grandchildA.transform);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            rootA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromAncestorsOf(grandchildA.transform);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        [Test]
        public void InjectsConcrete_IncludeSelfTrue()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent injectable = grandchildA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAncestorsOf(grandchildA.transform, includeSelf: true);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(injectable, requester.concreteComponent);
        }

        [Test]
        public void InjectsConcrete_IncludeSelfFalse()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            grandchildA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAncestorsOf(grandchildA.transform, includeSelf: false);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoAncestors()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            ComponentRequester requester = grandchildA.AddComponent<ComponentRequester>();
            TestScope scope = grandchildA.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromAncestorsOf(grandchildA.transform, includeSelf: false);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA = new GameObject();
            grandchildA = new GameObject();
            rootB = new GameObject();

            childA.transform.SetParent(rootA.transform);
            grandchildA.transform.SetParent(childA.transform);
        }
    }
}