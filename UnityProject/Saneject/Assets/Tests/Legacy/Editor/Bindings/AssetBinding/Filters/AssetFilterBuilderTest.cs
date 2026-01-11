using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Legacy.Runtime.Asset.AssetRequester;

namespace Tests.Legacy.Editor.Bindings.AssetBinding.Filters
{
    public class AssetFilterBuilderTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_UsingWhere()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .Where(asset => asset.name.StartsWith("Injectable") && asset.name.EndsWith("2"));

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.NotNull(requester.concreteAsset);
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_UsingWhere()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .Where(asset => asset.name.StartsWith("Injectable") && asset.name.EndsWith("2"));

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.NotNull(requester.interfaceAsset);
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}