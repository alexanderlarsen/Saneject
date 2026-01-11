using NUnit.Framework;
using Plugins.Saneject.Legacy.Editor.Core;
using Tests.Legacy.Runtime;
using Tests.Legacy.Runtime.Asset;
using UnityEditor;
using UnityEngine;
using AssetRequester = Tests.Legacy.Runtime.Asset.AssetRequester;

namespace Tests.Legacy.Editor.Bindings.AssetBinding.Locators.ProjectLoad
{
    public class FromFolderTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromFolder_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings: pick the one with name "... 1"
            BindAsset<InjectableScriptableObject>(scope)
                .FromFolder("Assets/Tests/Runtime/Resources/Test")
                .Where(a => a.name == "InjectableScriptableObject 1");

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            InjectableScriptableObject expected =
                AssetDatabase.LoadAssetAtPath<InjectableScriptableObject>(
                    "Assets/Tests/Runtime/Resources/Test/InjectableScriptableObject 1.asset");

            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromFolder_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings: pick the one with name "... 2"
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromFolder("Assets/Tests/Runtime/Resources/Test")
                .Where(a => a.name == "InjectableScriptableObject 2");

            // Inject
            DependencyInjector.InjectCurrentScene();

            // Assert
            InjectableScriptableObject expected =
                AssetDatabase.LoadAssetAtPath<InjectableScriptableObject>(
                    "Assets/Tests/Runtime/Resources/Test/InjectableScriptableObject 2.asset");

            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}