using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class AssetListTests
    {
        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void TryAddAssetByPath_GivenDuplicatePath_AddsAssetOnlyOnce()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("AssetList Prefab", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            AssetList assetList = new();

            try
            {
                // Add assets
                bool firstResult = assetList.TryAddAssetByPath<PrefabAssetData>(prefab.AssetPath);
                bool secondResult = assetList.TryAddAssetByPath<PrefabAssetData>(prefab.AssetPath);

                // Assert
                Assert.That(firstResult, Is.True);
                Assert.That(secondResult, Is.False);
                Assert.That(assetList.TotalCount, Is.EqualTo(1));
                Assert.That(assetList.GetElementAt(0).GetAssetPath(), Is.EqualTo(prefab.AssetPath));
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void GetEnabled_GivenMixedEnabledStates_ReturnsEnabledAssetsOnly()
        {
            // Set up prefab assets
            TestPrefabAsset firstPrefab = TestPrefabAsset.Create("Enabled Prefab", width: 1, depth: 1);
            TestPrefabAsset secondPrefab = TestPrefabAsset.Create("Disabled Prefab", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(firstPrefab.Root, firstPrefab.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(secondPrefab.Root, secondPrefab.AssetPath);
            AssetList assetList = new();

            try
            {
                // Add assets
                assetList.TryAddAssetByPath<PrefabAssetData>(firstPrefab.AssetPath);
                assetList.TryAddAssetByPath<PrefabAssetData>(secondPrefab.AssetPath);
                assetList.GetElementAt(1).Enabled = false;
                AssetData[] enabledAssets = assetList.GetEnabled();

                // Assert
                Assert.That(enabledAssets, Is.Not.Null);
                Assert.That(enabledAssets.Length, Is.EqualTo(1));
                CollectionAssert.AreEqual(new[]
                {
                    firstPrefab.AssetPath
                }, enabledAssets.Select(asset => asset.GetAssetPath()).ToArray());
            }
            finally
            {
                firstPrefab.Destroy();
                firstPrefab.DeleteAsset();
                secondPrefab.Destroy();
                secondPrefab.DeleteAsset();
            }
        }

        [Test]
        public void FindIndexByPath_GivenSortedList_ReturnsSortedIndex()
        {
            // Set up prefab assets
            TestPrefabAsset prefabTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabTen.Root, prefabTen.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabTwo.Root, prefabTwo.AssetPath);
            AssetList assetList = new();

            try
            {
                // Add assets
                assetList.TryAddAssetByPath<PrefabAssetData>(prefabTen.AssetPath);
                assetList.TryAddAssetByPath<PrefabAssetData>(prefabTwo.AssetPath);
                assetList.SortMode = SortMode.NameAtoZ;
                assetList.Sort();

                // Assert
                Assert.That(assetList.FindIndexByPath(prefabTwo.AssetPath), Is.EqualTo(0));
                Assert.That(assetList.FindIndexByPath(prefabTen.AssetPath), Is.EqualTo(1));
            }
            finally
            {
                prefabTen.Destroy();
                prefabTen.DeleteAsset();
                prefabTwo.Destroy();
                prefabTwo.DeleteAsset();
            }
        }

        [Test]
        public void FindGuidsNotInList_GivenExistingAndMissingGuids_ReturnsOnlyMissingGuids()
        {
            // Set up prefab assets
            TestPrefabAsset firstPrefab = TestPrefabAsset.Create("First Prefab", width: 1, depth: 1);
            TestPrefabAsset secondPrefab = TestPrefabAsset.Create("Second Prefab", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(firstPrefab.Root, firstPrefab.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(secondPrefab.Root, secondPrefab.AssetPath);
            AssetList assetList = new();

            try
            {
                // Add asset
                assetList.TryAddAssetByPath<PrefabAssetData>(firstPrefab.AssetPath);
                string[] missingGuids = assetList.FindGuidsNotInList(new[]
                {
                    AssetDatabase.AssetPathToGUID(firstPrefab.AssetPath),
                    AssetDatabase.AssetPathToGUID(secondPrefab.AssetPath)
                }).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    AssetDatabase.AssetPathToGUID(secondPrefab.AssetPath)
                }, missingGuids);
            }
            finally
            {
                firstPrefab.Destroy();
                firstPrefab.DeleteAsset();
                secondPrefab.Destroy();
                secondPrefab.DeleteAsset();
            }
        }

        [Test]
        public void Sort_GivenConfiguredSortMode_SortsList()
        {
            // Set up prefab assets
            TestPrefabAsset prefabA = TestPrefabAsset.Create("Prefab A", width: 1, depth: 1);
            TestPrefabAsset prefabB = TestPrefabAsset.Create("Prefab B", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabA.Root, prefabA.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabB.Root, prefabB.AssetPath);
            AssetList assetList = new();

            try
            {
                // Add assets
                assetList.TryAddAssetByPath<PrefabAssetData>(prefabA.AssetPath);
                assetList.TryAddAssetByPath<PrefabAssetData>(prefabB.AssetPath);
                assetList.SortMode = SortMode.NameZtoA;
                assetList.Sort();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab B",
                    "Prefab A"
                }, new[]
                {
                    assetList.GetElementAt(0).GetAssetName(),
                    assetList.GetElementAt(1).GetAssetName()
                });
            }
            finally
            {
                prefabA.Destroy();
                prefabA.DeleteAsset();
                prefabB.Destroy();
                prefabB.DeleteAsset();
            }
        }
    }
}
