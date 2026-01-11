using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Locators.CustomTarget
{
    public class FromFirstChildOfTest : BaseBindingTest
    {
        private GameObject rootA, childA1, childA2;
        private GameObject rootB;

        [Test]
        public void InjectsConcrete()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            childA1.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromFirstChildOf(rootA.transform);

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
            childA1.AddComponent<InjectableComponent>();
            ComponentRequester requester = rootB.AddComponent<ComponentRequester>();
            TestScope scope = rootB.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope).FromFirstChildOf(rootA.transform);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            rootA = new GameObject();
            childA1 = new GameObject();
            childA2 = new GameObject();
            rootB = new GameObject();

            childA1.transform.SetParent(rootA.transform);
            childA2.transform.SetParent(rootA.transform);
        }
    }
}