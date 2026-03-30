using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor.SceneManagement;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class PrefabListUtilityTests
    {
        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void AddAllPrefabsInScene_GivenPrefabInstances_AddsUniqueSortedPrefabsAndMarksDirty()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestPrefabAsset prefabTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            TestPrefabInstance firstPrefabTenInstance = prefabTen.Instantiate(scene.GetTransform("Root 1"), "Prefab 10 A");
            TestPrefabInstance secondPrefabTenInstance = prefabTen.Instantiate(scene.GetTransform("Root 1"), "Prefab 10 B");
            TestPrefabInstance prefabTwoInstance = prefabTwo.Instantiate(scene.GetTransform("Root 1"), "Prefab 2");
            prefabTen.Destroy();
            prefabTwo.Destroy();
            BatchInjectorData batchInjectorData = new();

            try
            {
                // Add prefabs
                PrefabListUtility.AddAllPrefabsInScene(batchInjectorData);

                // Assert
                Assert.That(batchInjectorData.IsDirty, Is.True);
                Assert.That(batchInjectorData.PrefabList.TotalCount, Is.EqualTo(2));
                CollectionAssert.AreEqual(new[]
                {
                    prefabTwo.AssetPath,
                    prefabTen.AssetPath
                }, Enumerable.Range(0, batchInjectorData.PrefabList.TotalCount)
                    .Select(index => batchInjectorData.PrefabList.GetElementAt(index).GetAssetPath())
                    .ToArray());
            }
            finally
            {
                firstPrefabTenInstance.Destroy();
                secondPrefabTenInstance.Destroy();
                prefabTwoInstance.Destroy();
                prefabTen.DeleteAsset();
                prefabTwo.DeleteAsset();
            }
        }

        [Test]
        public void AddAllPrefabsInScene_GivenExistingPrefabsInList_DoesNotDuplicateAssets()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab", width: 1, depth: 1);
            TestPrefabInstance prefabInstance = prefab.Instantiate(scene.GetTransform("Root 1"), "Prefab Instance");
            prefab.Destroy();
            BatchInjectorData batchInjectorData = new();

            try
            {
                // Add existing prefab
                batchInjectorData.PrefabList.TryAddAssetByPath<PrefabAssetData>(prefab.AssetPath);

                // Add prefabs
                PrefabListUtility.AddAllPrefabsInScene(batchInjectorData);

                // Assert
                Assert.That(batchInjectorData.IsDirty, Is.True);
                Assert.That(batchInjectorData.PrefabList.TotalCount, Is.EqualTo(1));
                CollectionAssert.AreEqual(new[]
                {
                    prefab.AssetPath
                }, new[]
                {
                    batchInjectorData.PrefabList.GetElementAt(0).GetAssetPath()
                });
            }
            finally
            {
                prefabInstance.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}
