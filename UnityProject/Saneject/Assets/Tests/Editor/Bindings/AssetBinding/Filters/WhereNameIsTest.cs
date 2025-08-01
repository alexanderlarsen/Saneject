using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Filters
{
    public class WhereNameIsTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_WhenNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .WhereNameIs("InjectableScriptableObject 1");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.NotNull(requester.concreteAsset);
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_WhenNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .WhereNameIs("InjectableScriptableObject 1");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 1");
            Assert.NotNull(requester.interfaceAsset);
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}