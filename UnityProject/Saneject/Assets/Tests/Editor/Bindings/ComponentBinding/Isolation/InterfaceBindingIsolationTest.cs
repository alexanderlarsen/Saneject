using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Component;
using UnityEngine;
using ComponentRequester = Tests.Runtime.Component.ComponentRequester;

namespace Tests.Editor.Bindings.ComponentBinding.Isolation
{
    public class InterfaceBindingIsolationTest : BaseBindingTest
    {
        private GameObject root, child, grandChild;

        [Test]
        public void InterfaceBindingDoesNotInjectConcreteField()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = child.AddComponent<ComponentRequester>();
            grandChild.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromDescendants();

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteComponent);
            Assert.NotNull(requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child = new GameObject();
            grandChild = new GameObject();

            child.transform.SetParent(root.transform);
            grandChild.transform.SetParent(child.transform);
        }
    }
}