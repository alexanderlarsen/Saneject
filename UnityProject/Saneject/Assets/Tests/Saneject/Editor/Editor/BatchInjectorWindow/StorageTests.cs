using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Persistence;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class StorageTests
    {
        private string fullPath;
        private string originalContents;
        private bool hadOriginalFile;

        [SetUp]
        public void SetUp()
        {
            fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/Saneject/BatchInjectorData.json"));
            hadOriginalFile = File.Exists(fullPath);
            originalContents = hadOriginalFile
                ? File.ReadAllText(fullPath)
                : null;

            if (hadOriginalFile)
                File.Delete(fullPath);
        }

        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            if (hadOriginalFile)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
                File.WriteAllText(fullPath, originalContents);
                return;
            }

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        [Test]
        public void LoadOrCreateData_GivenMissingFile_ReturnsDefaultData()
        {
            // Load data
            BatchInjectorData batchInjectorData = Storage.LoadOrCreateData();

            // Assert
            Assert.That(batchInjectorData, Is.Not.Null);
            Assert.That(batchInjectorData.SceneList.TotalCount, Is.EqualTo(0));
            Assert.That(batchInjectorData.PrefabList.TotalCount, Is.EqualTo(0));
            Assert.That(batchInjectorData.WindowTab, Is.EqualTo(WindowTab.Scenes));
            Assert.That(batchInjectorData.IsDirty, Is.False);
            Assert.That(File.Exists(fullPath), Is.False);
        }

        [Test]
        public void SaveIfDirty_GivenCleanData_DoesNotCreateFile()
        {
            // Set up data
            BatchInjectorData batchInjectorData = new();

            // Save
            Storage.SaveIfDirty(batchInjectorData);

            // Assert
            Assert.That(batchInjectorData.IsDirty, Is.False);
            Assert.That(File.Exists(fullPath), Is.False);
        }

        [Test]
        public void SaveIfDirty_GivenDirtyData_SavesDataAndClearsDirtyFlag()
        {
            // Set up assets
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string scenePath = scene.SaveToDisk();
            TestPrefabAsset prefab = TestPrefabAsset.Create("Storage Prefab", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            BatchInjectorData batchInjectorData = new();

            try
            {
                // Add assets
                batchInjectorData.SceneList.TryAddAssetByPath<SceneAssetData>(scenePath);
                batchInjectorData.PrefabList.TryAddAssetByPath<PrefabAssetData>(prefab.AssetPath);
                SceneAssetData sceneAssetData = batchInjectorData.SceneList.GetElementAt(0) as SceneAssetData;
                PrefabAssetData prefabAssetData = batchInjectorData.PrefabList.GetElementAt(0) as PrefabAssetData;

                // Configure data
                Assert.That(sceneAssetData, Is.Not.Null);
                Assert.That(prefabAssetData, Is.Not.Null);

                sceneAssetData.Enabled = false;
                sceneAssetData.Status = InjectionStatus.Warning;
                sceneAssetData.ContextWalkFilter = ContextWalkFilter.SceneObjects;
                prefabAssetData.Status = InjectionStatus.Error;
                prefabAssetData.ContextWalkFilter = ContextWalkFilter.PrefabInstances;
                batchInjectorData.SceneList.SortMode = SortMode.PathZtoA;
                batchInjectorData.PrefabList.SortMode = SortMode.NameZtoA;
                batchInjectorData.SceneList.Scroll = new Vector2(13f, 17f);
                batchInjectorData.PrefabList.Scroll = new Vector2(19f, 23f);
                batchInjectorData.WindowTab = WindowTab.Prefabs;
                batchInjectorData.IsDirty = true;

                // Save
                Storage.SaveIfDirty(batchInjectorData);
                BatchInjectorData loadedData = Storage.LoadOrCreateData();
                SceneAssetData loadedSceneAssetData = loadedData.SceneList.GetElementAt(0) as SceneAssetData;
                PrefabAssetData loadedPrefabAssetData = loadedData.PrefabList.GetElementAt(0) as PrefabAssetData;

                // Assert
                Assert.That(batchInjectorData.IsDirty, Is.False);
                Assert.That(File.Exists(fullPath), Is.True);
                Assert.That(loadedData, Is.Not.Null);
                Assert.That(loadedData.SceneList.TotalCount, Is.EqualTo(1));
                Assert.That(loadedData.PrefabList.TotalCount, Is.EqualTo(1));
                Assert.That(loadedData.WindowTab, Is.EqualTo(WindowTab.Prefabs));
                Assert.That(loadedSceneAssetData, Is.Not.Null);
                Assert.That(loadedPrefabAssetData, Is.Not.Null);
                Assert.That(loadedSceneAssetData.GetAssetPath(), Is.EqualTo(scenePath));
                Assert.That(loadedSceneAssetData.Enabled, Is.False);
                Assert.That(loadedSceneAssetData.Status, Is.EqualTo(InjectionStatus.Warning));
                Assert.That(loadedSceneAssetData.ContextWalkFilter, Is.EqualTo(ContextWalkFilter.SceneObjects));
                Assert.That(loadedData.SceneList.SortMode, Is.EqualTo(SortMode.PathZtoA));
                Assert.That(loadedData.SceneList.Scroll, Is.EqualTo(new Vector2(13f, 17f)));
                Assert.That(loadedPrefabAssetData.GetAssetPath(), Is.EqualTo(prefab.AssetPath));
                Assert.That(loadedPrefabAssetData.Enabled, Is.True);
                Assert.That(loadedPrefabAssetData.Status, Is.EqualTo(InjectionStatus.Error));
                Assert.That(loadedPrefabAssetData.ContextWalkFilter, Is.EqualTo(ContextWalkFilter.PrefabInstances));
                Assert.That(loadedData.PrefabList.SortMode, Is.EqualTo(SortMode.NameZtoA));
                Assert.That(loadedData.PrefabList.Scroll, Is.EqualTo(new Vector2(19f, 23f)));
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                scene.DeleteFromDisk();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void LoadOrCreateData_GivenInvalidJson_ReturnsDefaultDataAndRecreatesFile()
        {
            // Write invalid file
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
            File.WriteAllText(fullPath, "{ invalid json");
            LogAssert.Expect(LogType.Error, new Regex("^Saneject: Batch Injector failed to load configuration data\\..*"));

            // Load data
            BatchInjectorData batchInjectorData = Storage.LoadOrCreateData();
            string recreatedJson = File.ReadAllText(fullPath);

            // Assert
            Assert.That(batchInjectorData, Is.Not.Null);
            Assert.That(batchInjectorData.SceneList.TotalCount, Is.EqualTo(0));
            Assert.That(batchInjectorData.PrefabList.TotalCount, Is.EqualTo(0));
            Assert.That(batchInjectorData.WindowTab, Is.EqualTo(WindowTab.Scenes));
            Assert.That(batchInjectorData.IsDirty, Is.False);
            Assert.That(File.Exists(fullPath), Is.True);
            Assert.That(recreatedJson, Is.Not.EqualTo("{ invalid json"));
            Assert.That(recreatedJson, Does.Contain("windowTab"));
        }
    }
}
