using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Filters
{
    public class WhereNameContainsTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_WhenNameContains()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .WhereNameContains("Object 1");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.AreEqual(expected, requester.concreteAsset);
            Assert.NotNull(requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_WhenNameContains()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .WhereNameContains("Object 2");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.AreEqual(expected, requester.interfaceAsset);
            Assert.NotNull(requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}