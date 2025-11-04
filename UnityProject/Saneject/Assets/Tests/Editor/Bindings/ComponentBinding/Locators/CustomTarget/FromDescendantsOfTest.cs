using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.CustomTarget
{
    public class FromDescendantsOfTest : BaseBindingTest
    {
        private GameObject rootA, childA, grandchildA;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            grandchildA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromDescendantsOf(rootA.transform);

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
            grandchildA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromDescendantsOf(rootA.transform);

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
            InjectableComponent injectable = rootA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromDescendantsOf(rootA.transform, includeSelf: true);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(injectable, requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenOnlySelfPresentAndIncludeSelfFalse()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            rootA.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromDescendantsOf(rootA.transform, includeSelf: false);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
        }

        [Test]
        public void DoesNotInject_WhenNoDescendants()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromDescendantsOf(rootA.transform, includeSelf: false);

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