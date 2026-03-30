using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class SortingUtilityTests
    {
        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void SortList_GivenNameAtoZ_SortsNaturallyByName()
        {
            // Set up prefab assets
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            TestPrefabAsset prefabTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabOne = TestPrefabAsset.Create("Prefab 1", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabTwo.Root, prefabTwo.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabTen.Root, prefabTen.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabOne.Root, prefabOne.AssetPath);

            try
            {
                // Sort list
                List<AssetData> list = new()
                {
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabTwo.AssetPath)),
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabTen.AssetPath)),
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabOne.AssetPath))
                };

                SortingUtility.SortList(list, SortMode.NameAtoZ);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab 1",
                    "Prefab 2",
                    "Prefab 10"
                }, list.Select(asset => asset.GetAssetName()).ToArray());
            }
            finally
            {
                prefabTwo.Destroy();
                prefabTwo.DeleteAsset();
                prefabTen.Destroy();
                prefabTen.DeleteAsset();
                prefabOne.Destroy();
                prefabOne.DeleteAsset();
            }
        }

        [Test]
        public void SortList_GivenPathZtoA_SortsByPathDescending()
        {
            // Set up prefab assets
            TestPrefabAsset prefabA = TestPrefabAsset.Create("Path A Prefab", width: 1, depth: 1);
            TestPrefabAsset prefabB = TestPrefabAsset.Create("Path B Prefab", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabA.Root, prefabA.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabB.Root, prefabB.AssetPath);

            try
            {
                // Sort list
                List<AssetData> list = new()
                {
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabA.AssetPath)),
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabB.AssetPath))
                };

                SortingUtility.SortList(list, SortMode.PathZtoA);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    prefabB.AssetPath,
                    prefabA.AssetPath
                }, list.Select(asset => asset.GetAssetPath()).ToArray());
            }
            finally
            {
                prefabA.Destroy();
                prefabA.DeleteAsset();
                prefabB.Destroy();
                prefabB.DeleteAsset();
            }
        }

        [Test]
        public void SortList_GivenEnabledToDisabled_SortsByEnabledThenName()
        {
            // Set up prefab assets
            TestPrefabAsset prefabTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            TestPrefabAsset prefabOne = TestPrefabAsset.Create("Prefab 1", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabTen.Root, prefabTen.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabTwo.Root, prefabTwo.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabOne.Root, prefabOne.AssetPath);

            try
            {
                // Sort list
                PrefabAssetData assetTen = new(AssetDatabase.AssetPathToGUID(prefabTen.AssetPath));
                PrefabAssetData assetTwo = new(AssetDatabase.AssetPathToGUID(prefabTwo.AssetPath));
                PrefabAssetData assetOne = new(AssetDatabase.AssetPathToGUID(prefabOne.AssetPath));
                assetTen.Enabled = true;
                assetTwo.Enabled = true;
                assetOne.Enabled = false;
                List<AssetData> list = new()
                {
                    assetTen,
                    assetOne,
                    assetTwo
                };

                SortingUtility.SortList(list, SortMode.EnabledToDisabled);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab 2",
                    "Prefab 10",
                    "Prefab 1"
                }, list.Select(asset => asset.GetAssetName()).ToArray());
            }
            finally
            {
                prefabTen.Destroy();
                prefabTen.DeleteAsset();
                prefabTwo.Destroy();
                prefabTwo.DeleteAsset();
                prefabOne.Destroy();
                prefabOne.DeleteAsset();
            }
        }

        [Test]
        public void SortList_GivenDisabledToEnabled_SortsByDisabledThenName()
        {
            // Set up prefab assets
            TestPrefabAsset prefabTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            TestPrefabAsset prefabOne = TestPrefabAsset.Create("Prefab 1", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabTen.Root, prefabTen.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabTwo.Root, prefabTwo.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabOne.Root, prefabOne.AssetPath);

            try
            {
                // Sort list
                PrefabAssetData assetTen = new(AssetDatabase.AssetPathToGUID(prefabTen.AssetPath));
                PrefabAssetData assetTwo = new(AssetDatabase.AssetPathToGUID(prefabTwo.AssetPath));
                PrefabAssetData assetOne = new(AssetDatabase.AssetPathToGUID(prefabOne.AssetPath));
                assetTen.Enabled = true;
                assetTwo.Enabled = false;
                assetOne.Enabled = false;
                List<AssetData> list = new()
                {
                    assetTen,
                    assetOne,
                    assetTwo
                };

                SortingUtility.SortList(list, SortMode.DisabledToEnabled);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab 1",
                    "Prefab 2",
                    "Prefab 10"
                }, list.Select(asset => asset.GetAssetName()).ToArray());
            }
            finally
            {
                prefabTen.Destroy();
                prefabTen.DeleteAsset();
                prefabTwo.Destroy();
                prefabTwo.DeleteAsset();
                prefabOne.Destroy();
                prefabOne.DeleteAsset();
            }
        }

        [Test]
        public void SortList_GivenStatusErrorToSuccess_SortsByStatusThenName()
        {
            // Set up prefab assets
            TestPrefabAsset prefabError = TestPrefabAsset.Create("Prefab Error", width: 1, depth: 1);
            TestPrefabAsset prefabWarningTen = TestPrefabAsset.Create("Prefab 10", width: 1, depth: 1);
            TestPrefabAsset prefabWarningTwo = TestPrefabAsset.Create("Prefab 2", width: 1, depth: 1);
            TestPrefabAsset prefabSuccess = TestPrefabAsset.Create("Prefab Success", width: 1, depth: 1);
            TestPrefabAsset prefabUnknown = TestPrefabAsset.Create("Prefab Unknown", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabError.Root, prefabError.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabWarningTen.Root, prefabWarningTen.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabWarningTwo.Root, prefabWarningTwo.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabSuccess.Root, prefabSuccess.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabUnknown.Root, prefabUnknown.AssetPath);

            try
            {
                // Sort list
                PrefabAssetData errorAsset = new(AssetDatabase.AssetPathToGUID(prefabError.AssetPath));
                PrefabAssetData warningTenAsset = new(AssetDatabase.AssetPathToGUID(prefabWarningTen.AssetPath));
                PrefabAssetData warningTwoAsset = new(AssetDatabase.AssetPathToGUID(prefabWarningTwo.AssetPath));
                PrefabAssetData successAsset = new(AssetDatabase.AssetPathToGUID(prefabSuccess.AssetPath));
                PrefabAssetData unknownAsset = new(AssetDatabase.AssetPathToGUID(prefabUnknown.AssetPath));
                errorAsset.Status = InjectionStatus.Error;
                warningTenAsset.Status = InjectionStatus.Warning;
                warningTwoAsset.Status = InjectionStatus.Warning;
                successAsset.Status = InjectionStatus.Success;
                unknownAsset.Status = InjectionStatus.Unknown;
                List<AssetData> list = new()
                {
                    successAsset,
                    warningTenAsset,
                    unknownAsset,
                    errorAsset,
                    warningTwoAsset
                };

                SortingUtility.SortList(list, SortMode.StatusErrorToSuccess);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab Error",
                    "Prefab 2",
                    "Prefab 10",
                    "Prefab Success",
                    "Prefab Unknown"
                }, list.Select(asset => asset.GetAssetName()).ToArray());
            }
            finally
            {
                prefabError.Destroy();
                prefabError.DeleteAsset();
                prefabWarningTen.Destroy();
                prefabWarningTen.DeleteAsset();
                prefabWarningTwo.Destroy();
                prefabWarningTwo.DeleteAsset();
                prefabSuccess.Destroy();
                prefabSuccess.DeleteAsset();
                prefabUnknown.Destroy();
                prefabUnknown.DeleteAsset();
            }
        }

        [Test]
        public void SortList_GivenCustomMode_DoesNotChangeOrder()
        {
            // Set up prefab assets
            TestPrefabAsset prefabA = TestPrefabAsset.Create("Prefab A", width: 1, depth: 1);
            TestPrefabAsset prefabB = TestPrefabAsset.Create("Prefab B", width: 1, depth: 1);
            TestPrefabAsset prefabC = TestPrefabAsset.Create("Prefab C", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefabA.Root, prefabA.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabB.Root, prefabB.AssetPath);
            PrefabUtility.SaveAsPrefabAsset(prefabC.Root, prefabC.AssetPath);

            try
            {
                // Sort list
                List<AssetData> list = new()
                {
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabB.AssetPath)),
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabC.AssetPath)),
                    new PrefabAssetData(AssetDatabase.AssetPathToGUID(prefabA.AssetPath))
                };

                SortingUtility.SortList(list, SortMode.Custom);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    "Prefab B",
                    "Prefab C",
                    "Prefab A"
                }, list.Select(asset => asset.GetAssetName()).ToArray());
            }
            finally
            {
                prefabA.Destroy();
                prefabA.DeleteAsset();
                prefabB.Destroy();
                prefabB.DeleteAsset();
                prefabC.Destroy();
                prefabC.DeleteAsset();
            }
        }
    }
}
