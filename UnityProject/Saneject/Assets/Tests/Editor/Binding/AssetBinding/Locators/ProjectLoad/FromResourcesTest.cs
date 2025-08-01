using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Locators.ProjectLoad
{
    public class FromResourcesTest : BaseTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromResources()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope).FromResources("Test/InjectableScriptableObject 1");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromResources()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope).FromResources("Test/InjectableScriptableObject 2");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}