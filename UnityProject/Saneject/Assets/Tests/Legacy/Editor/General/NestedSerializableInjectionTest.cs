using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Component;
using Tests.Legacy.Runtime.NestedSerializable;
using UnityEngine;

namespace Tests.Legacy.Editor.General
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