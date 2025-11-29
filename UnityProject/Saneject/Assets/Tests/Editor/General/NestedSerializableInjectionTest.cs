using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using Tests.Runtime.NestedSerializable;
using UnityEngine;

namespace Tests.Editor.General
{
    public class NestedSerializableInjectionTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsIntoNestedSerializableClass()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            RequesterWithNestedSerializable requester = root.AddComponent<RequesterWithNestedSerializable>();
            InjectableComponent injectable = root.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope).FromSelf();
            BindComponent<IInjectable, InjectableComponent>(scope).FromSelf();

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.NotNull(requester.nested);
            Assert.AreEqual(injectable, requester.nested.concrete);
            Assert.AreEqual(injectable, requester.nested.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}