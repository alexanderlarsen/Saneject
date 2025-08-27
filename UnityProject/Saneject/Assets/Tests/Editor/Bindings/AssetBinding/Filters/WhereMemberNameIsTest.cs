using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Filters
{
    public class WhereMemberNameIsTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcreteAsset_WhenMemberNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject asset = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromInstance(asset)
                .WhereMemberNameIs(nameof(AssetRequester.concreteAsset));

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(asset, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterfaceAsset_WhenMemberNameMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject asset = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromInstance(asset)
                .WhereMemberNameIs(nameof(AssetRequester.interfaceAsset));

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.AreEqual(asset, requester.interfaceAsset);
        }

        [Test]
        public void DoesNotInject_WhenMemberNameDoesNotMatch()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject asset = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings (filter does not match any member)
            BindAsset<InjectableScriptableObject>(scope)
                .FromInstance(asset)
                .WhereMemberNameIs("NonExistentMember");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNull(requester.concreteAsset);
            Assert.IsNull(requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}