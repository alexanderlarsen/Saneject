using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Locators.ProjectLoad
{
    public class FromResourcesAllTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromResourcesAll_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .Where(a => a.name == "InjectableScriptableObject 2");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromResourcesAll_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .Where(a => a.name == "InjectableScriptableObject 3");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 3");
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}