using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequesterWithID = Tests.Runtime.Asset.AssetRequesterWithID;

namespace Tests.Editor.Bindings.AssetBinding.Qualifiers
{
    public class ToIdTest : BaseBindingTest
    {
        private GameObject root, child1, child2;

        [Test]
        public void InjectsConcreteAssetByID()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequesterWithID requester = root.AddComponent<AssetRequesterWithID>();
            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope).ToID("componentA").FromInstance(assetA);
            BindAsset<InjectableScriptableObject>(scope).ToID("componentB").FromInstance(assetB);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.concreteComponentA);
            Assert.NotNull(requester.concreteComponentB);
            Assert.AreEqual(assetA, requester.concreteComponentA);
            Assert.AreEqual(assetB, requester.concreteComponentB);
            Assert.AreNotEqual(requester.concreteComponentA, requester.concreteComponentB);
        }

        [Test]
        public void InjectsInterfaceAssetByID()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequesterWithID requester = root.AddComponent<AssetRequesterWithID>();
            InjectableScriptableObject assetA = ScriptableObject.CreateInstance<InjectableScriptableObject>();
            InjectableScriptableObject assetB = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope).ToID("componentA").FromInstance(assetA);
            BindAsset<IInjectable, InjectableScriptableObject>(scope).ToID("componentB").FromInstance(assetB);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.NotNull(requester.interfaceComponentA);
            Assert.NotNull(requester.interfaceComponentB);
            Assert.AreEqual(assetA, requester.interfaceComponentA);
            Assert.AreEqual(assetB, requester.interfaceComponentB);
            Assert.AreNotEqual(requester.interfaceComponentA, requester.interfaceComponentB);
        }

        [Test]
        public void NonIdBindingDoesNotInjectIntoIdAssetField()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequesterWithID requester = root.AddComponent<AssetRequesterWithID>();
            InjectableScriptableObject asset = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up a binding WITHOUT ID
            BindAsset<InjectableScriptableObject>(scope).FromInstance(asset);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert – ID fields should remain null because no matching ID binding exists
            Assert.IsNull(requester.concreteComponentA);
            Assert.IsNull(requester.concreteComponentB);
            Assert.IsNull(requester.interfaceComponentA);
            Assert.IsNull(requester.interfaceComponentB);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
            child1 = new GameObject();
            child2 = new GameObject();

            child1.transform.SetParent(root.transform);
            child2.transform.SetParent(root.transform);
        }
    }
}