using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using Tests.Runtime.Asset;
using UnityEngine;
using AssetRequester = Tests.Runtime.Asset.AssetRequester;

namespace Tests.Editor.Bindings.AssetBinding.Filters
{
    public class ComplexFilterTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void InjectsOnlyWhenAllFiltersPass()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            AssetRequester requester = root.AddComponent<AssetRequester>();

            // Set up bindings
            BindAsset<InjectableScriptableObject>(scope)
                .FromResourcesAll("Test")
                .WhereNameContains("ScriptableObject") // "InjectableScriptableObject 2"
                .WhereNameIs("InjectableScriptableObject 2")
                .WhereTargetIs<AssetRequester>()
                .Where(asset => !asset.name.EndsWith("1") && !asset.name.EndsWith("3"));

            // Inject 
            DependencyInjector.InjectSceneDependencies();

            // Assert
            InjectableScriptableObject expected = Resources.Load<InjectableScriptableObject>("Test/InjectableScriptableObject 2");
            Assert.AreEqual(expected, requester.concreteAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}