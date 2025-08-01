using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Locators.ProjectLoad
{
    public class FromAssetLoadAllTest : BaseTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromAssetLoadAll_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromAssetLoadAll("Assets/Tests/Resources/Test/InjectableScriptableObject 1.asset")
                .WhereNameIs("InjectableScriptableObject 1");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = AssetDatabase.LoadAssetAtPath<InjectableScriptableObject>("Assets/Tests/Resources/Test/InjectableScriptableObject 1.asset");
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromAssetLoadAll_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<IInjectable, InjectableScriptableObject>(scope)
                .FromAssetLoadAll("Assets/Tests/Resources/Test/InjectableScriptableObject 2.asset")
                .WhereNameIs("InjectableScriptableObject 2");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = AssetDatabase.LoadAssetAtPath<InjectableScriptableObject>("Assets/Tests/Resources/Test/InjectableScriptableObject 2.asset");
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject("Root");
        }
    }
}