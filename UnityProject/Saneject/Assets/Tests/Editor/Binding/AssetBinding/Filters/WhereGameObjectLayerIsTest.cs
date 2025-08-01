using NUnit.Framework;
using Plugins.Saneject.Editor.Core;
using Tests.Runtime;
using UnityEngine;

namespace Tests.Editor.Binding.AssetBinding.Filters
{
    public class WhereGameObjectLayerIsTest : BaseBindingTest
    {
        private GameObject root;

        [Test]
        public void Injects_WhenLayerMatches()
        {
            // Suppress errors from unbound dependencies
            IgnoreErrorMessages();

            // Add components
            TestScope scope = root.AddComponent<TestScope>();
            GameObjectRequester requester = root.AddComponent<GameObjectRequester>();

            // Set up bindings
            BindAsset<GameObject>(scope)
                .FromResourcesAll("Test")
                .WhereGameObjectLayerIs(LayerMask.NameToLayer("Test"));

            // Inject
            DependencyInjector.InjectSceneDependencies();

            // Assert
            GameObject expected = Resources.Load<GameObject>("Test/Prefab 3");
            Assert.NotNull(requester.gameObjectAsset);
            Assert.AreEqual(expected, requester.gameObjectAsset);
        }

        protected override void CreateHierarchy()
        {
            root = new GameObject();
        }
    }
}