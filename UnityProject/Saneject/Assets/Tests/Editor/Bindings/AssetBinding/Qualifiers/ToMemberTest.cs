using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Qualifiers
{
    public class ToMemberTest : BaseBindingTest
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
                .ToMember(nameof(AssetRequester.concreteAsset))
                .FromInstance(asset);

            // Inject
            DependencyInjector.InjectCurrentScene();

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
                .ToMember(nameof(AssetRequester.interfaceAsset))
                .FromInstance(asset);

            // Inject
            DependencyInjector.InjectCurrentScene();

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
                .ToMember("NonExistentMember")
                .FromInstance(asset);

            // Inject
            DependencyInjector.InjectCurrentScene();

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