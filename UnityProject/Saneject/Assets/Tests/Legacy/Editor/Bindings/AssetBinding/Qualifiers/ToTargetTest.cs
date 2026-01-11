using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Legacy.Runtime.Asset.AssetRequester;
using AssetRequesterB = Tests.Legacy.Runtime.Asset.AssetRequesterB;

namespace Tests.Legacy.Editor.Bindings.AssetBinding.Qualifiers
{
    public class ToTargetTest : BaseBindingTest
    {
        private GameObject root, child1, child2, child3, child4, child5, child6;

        [Test]
        public void InjectsAssetOnlyIntoMatchingTarget()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requesterA = child3.AddComponent<AssetRequester>();
            AssetRequesterB requesterB = child4.AddComponent<AssetRequesterB>();
            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .ToTarget<AssetRequester>()
                .FromInstance(assetA);

            BindAsset<InjectableScriptableObject>(scope)
                .ToTarget<AssetRequesterB>()
                .FromInstance(assetB);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(assetA, requesterA.concreteAsset);
            Assert.AreEqual(assetB, requesterB.concreteAsset);
        }

        [Test]
        public void InjectsInterfaceAssetOnlyIntoMatchingTarget()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requesterA = child5.AddComponent<AssetRequester>();
            AssetRequesterB requesterB = child6.AddComponent<AssetRequesterB>();
            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .ToTarget<AssetRequester>()
                .FromInstance(assetA);

            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .ToTarget<AssetRequesterB>()
                .FromInstance(assetB);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(assetA, requesterA.interfaceAsset);
            Assert.AreEqual(assetB, requesterB.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();
            child3 = new GameObject();
            child4 = new GameObject();
            child5 = new GameObject();
            child6 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
            child3.transform.SetParent(root.transform);
            child4.transform.SetParent(root.transform);
            child5.transform.SetParent(root.transform);
            child6.transform.SetParent(root.transform);
        }
    }
}