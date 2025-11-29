using System.Collections.Generic;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Locators.Special
{
    public class FromMethodCollectionTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromMethodCollection_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetA.name = "A";

            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetB.name = "B";

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromMethod(() => new[] { assetA, assetB })
                .Where(a => a.name == "B");

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(assetB, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromMethodCollection_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetA.name = "A";

            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            assetB.name = "B";

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromMethod(() => new List<InjectableScriptableObject> { assetA, assetB })
                .Where(a => a.name == "B");

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(assetB, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}