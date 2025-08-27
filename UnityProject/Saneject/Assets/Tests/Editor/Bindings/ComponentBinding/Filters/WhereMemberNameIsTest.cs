using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Filters
{
    public class WhereMemberNameIsTest : BaseBindingTest
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
                .FromInstance(target)
                .WhereMemberNameIs(nameof(ComponentRequester.concreteComponent));

            // Inject
            DependencyInjector.InjectSceneDependencies();

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
                .FromInstance(target)
                .WhereMemberNameIs(nameof(ComponentRequester.interfaceComponent));

            // Inject
            DependencyInjector.InjectSceneDependencies();

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
                .FromInstance(target)
                .WhereMemberNameIs("NonExistentMember");

            // Inject
            DependencyInjector.InjectSceneDependencies();

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