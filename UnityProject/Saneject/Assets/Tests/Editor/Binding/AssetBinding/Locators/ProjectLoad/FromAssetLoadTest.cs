#if UNITY_EDITOR
using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Locators.ProjectLoad
{
    public class FromAssetLoadTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_FromAssetLoad_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            scope.BindAsset<InjectableScriptableObject>().FromAssetLoad("Assets/Tests/Resources/Test/InjectableScriptableObject 1.asset");

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = AssetDatabase.LoadAssetAtPath<InjectableScriptableObject>("Assets/Tests/Resources/Test/InjectableScriptableObject 1.asset");
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_FromAssetLoad_WithFilter()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            scope.BindAsset<IInjectable, InjectableScriptableObject>().FromAssetLoad("Assets/Tests/Resources/Test/InjectableScriptableObject 2.asset");

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
#endif