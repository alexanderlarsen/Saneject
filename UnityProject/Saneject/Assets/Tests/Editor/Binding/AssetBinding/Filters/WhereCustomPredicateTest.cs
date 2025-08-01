using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Filters
{
    public class WhereCustomPredicateTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsConcrete_WithCustomPredicate()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            scope.BindAsset<InjectableScriptableObject>()
                .FromResourcesAll("Test")
                .Where(asset => asset.name.StartsWith("Injectable") && asset.name.EndsWith("2"));

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.NotNull(requester.concreteAsset);
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        [Test]
        public void InjectsInterface_WithCustomPredicate()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            scope.BindAsset<IInjectable, InjectableScriptableObject>()
                .FromResourcesAll("Test")
                .Where(asset => asset.name.StartsWith("Injectable") && asset.name.EndsWith("2"));

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.NotNull(requester.interfaceAsset);
            Assert.AreEqual(expected, requester.interfaceAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}