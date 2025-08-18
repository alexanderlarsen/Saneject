using System.IO;
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Settings;
using Tests.Runtime;
using Tests.Runtime.Component;
using Tests.Runtime.Proxy;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.Bindings.ComponentBinding.Locators.Special
{
    public class FromProxyTest : BaseBindingTest
    {
        // Script stub creation is untested. I don't how how to make a unit test that survives domain reload, especially in CI.

        private GameObject root;

        [Test]
        public void CreatesAndInjectsProxyObject()
        {
            string prevFolder = UserSettings.ProxyAssetGenerationFolder;
            UserSettings.ProxyAssetGenerationFolder = "Assets/Tests/Runtime/Resources/Generated";

            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            ProxyRequester requester = root.AddComponent<ProxyRequester>();
            TestScope scope = root.AddComponent<TestScope>();

            // Set up bindings
            BindComponent<IProxyTarget, ProxyTarget>(scope).FromProxy();

            // Delete existing proxy asset
            string path = Path.Combine(UserSettings.ProxyAssetGenerationFolder, "ProxyTargetProxy.asset");

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            bool assetExists = AssetDatabase.LoadAssetAtPath<Object>(path) != null;
            Assert.IsFalse(assetExists);

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            Assert.IsNotNull(requester.proxyTarget);

            UserSettings.ProxyAssetGenerationFolder = prevFolder;
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}