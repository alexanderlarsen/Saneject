using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.ComponentBinding.Filters
{
    public class WhereLayerIsTest : BaseBindingTest
    {
        private GameObject root, layerMatchChild, otherLayerChild;

        [Test]
        public void InjectsConcrete_WhenLayerMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            layerMatchChild.layer = 8;
            InjectableComponent component = layerMatchChild.AddComponent<InjectableComponent>();
            otherLayerChild.layer = 0;
            otherLayerChild.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereLayerIs(8);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.concreteComponent);
        }

        [Test]
        public void InjectsInterface_WhenLayerMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            ComponentRequester requester = root.AddComponent<ComponentRequester>();
            layerMatchChild.layer = 8;
            InjectableComponent component = layerMatchChild.AddComponent<InjectableComponent>();
            otherLayerChild.layer = 0;
            otherLayerChild.AddComponent<InjectableComponent>();

            // Set up bindings
            BindComponent<IInjectable, InjectableComponent>(scope)
                .FromRootDescendants()
                .WhereLayerIs(8);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(component, requester.interfaceComponent);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            layerMatchChild = new GameObject();
            otherLayerChild = new GameObject();

            layerMatchChild.transform.SetParent(root.transform);
            otherLayerChild.transform.SetParent(root.transform);
        }
    }
}