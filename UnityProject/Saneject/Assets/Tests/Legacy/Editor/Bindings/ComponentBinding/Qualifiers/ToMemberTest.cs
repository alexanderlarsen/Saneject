using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Legacy.Runtime.Component.ComponentRequester;

namespace Tests.Legacy.Editor.Bindings.ComponentBinding.Qualifiers
{
    public class ToMemberTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_WhenMemberNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent target = root.AddComponent<InjectableComponent>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .ToMember(nameof(ComponentRequester.concreteComponent))
                .FromInstance(target);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(target, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenMemberNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent target = root.AddComponent<InjectableComponent>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .ToMember(nameof(ComponentRequester.interfaceComponent))
                .FromInstance(target);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(target, requester.interfaceComponent);
        }

        [Test]
        public void DoesNotInject_WhenMemberNameDoesNotMatch()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            InjectableComponent target = root.AddComponent<InjectableComponent>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings (filter does not match any member)
            BindComponent<InjectableComponent>(scope)
                .ToMember("NonExistentMember")
                .FromInstance(target);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.IsNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}