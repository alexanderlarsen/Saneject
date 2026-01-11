using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Legacy.Runtime.Asset.AssetRequester;

namespace Tests.Legacy.Editor.Bindings.AssetBinding.Locators.Special
{
    public class FromMethodSingleTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromMethod()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject instance = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope).FromMethod(() => instance);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(instance, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromMethod()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();
            InjectableScriptableObject instance = ScriptableObject.CreateInstance<InjectableScriptableObject>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope).FromMethod(() => instance);

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            Assert.AreEqual(instance, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}