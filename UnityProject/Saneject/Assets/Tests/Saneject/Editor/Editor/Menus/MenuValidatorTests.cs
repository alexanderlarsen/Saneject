using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Plugins.Saneject.Editor.Menus.SanejectMenuItems;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Menus
{
    public class MenuValidatorTests
    {
        [TearDown]
        public void TearDown()
        {
            Selection.objects = Array.Empty<Object>();

            TestPrefabAsset.CloseStage();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void IsScene_GivenScene_ReturnsTrue()
        {
            // Set up scene
            TestScene.Create(roots: 1, width: 1, depth: 1);
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo isSceneMethod = menuValidatorType?.GetMethod("IsScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo isPrefabStageMethod = menuValidatorType?.GetMethod("IsPrefabStage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            // Assert
            Assert.That(menuValidatorType, Is.Not.Null);
            Assert.That(isSceneMethod, Is.Not.Null);
            Assert.That(isPrefabStageMethod, Is.Not.Null);
            Assert.That((bool)isSceneMethod.Invoke(null, null), Is.True);
            Assert.That((bool)isPrefabStageMethod.Invoke(null, null), Is.False);
        }

        [Test]
        public void IsPrefabStage_GivenPrefabStage_ReturnsTrue()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance prefabStage = prefab.OpenStage();
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo isSceneMethod = menuValidatorType?.GetMethod("IsScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo isPrefabStageMethod = menuValidatorType?.GetMethod("IsPrefabStage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Assert
                Assert.That(prefabStage, Is.Not.Null);
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(isSceneMethod, Is.Not.Null);
                Assert.That(isPrefabStageMethod, Is.Not.Null);
                Assert.That((bool)isSceneMethod.Invoke(null, null), Is.False);
                Assert.That((bool)isPrefabStageMethod.Invoke(null, null), Is.True);
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void GetSceneObjectSelectionCount_GivenMixedSceneAndPrefabAssetSelection_CountsSceneObjectsOnly()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo getSceneObjectSelectionCountMethod = menuValidatorType?.GetMethod("GetSceneObjectSelectionCount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo hasSceneObjectSelectionMethod = menuValidatorType?.GetMethod("HasSceneObjectSelection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Select objects
                Selection.objects = new Object[]
                {
                    scene.GetTransform("Root 1").gameObject,
                    prefabAsset
                };

                // Assert
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(getSceneObjectSelectionCountMethod, Is.Not.Null);
                Assert.That(hasSceneObjectSelectionMethod, Is.Not.Null);
                Assert.That((int)getSceneObjectSelectionCountMethod.Invoke(null, null), Is.EqualTo(1));
                Assert.That((bool)hasSceneObjectSelectionMethod.Invoke(null, null), Is.True);
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void HasSceneObjectSelection_GivenOnlyPrefabAssetSelection_ReturnsFalse()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo getSceneObjectSelectionCountMethod = menuValidatorType?.GetMethod("GetSceneObjectSelectionCount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo hasSceneObjectSelectionMethod = menuValidatorType?.GetMethod("HasSceneObjectSelection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Select asset
                Selection.objects = new Object[] { prefabAsset };

                // Assert
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(getSceneObjectSelectionCountMethod, Is.Not.Null);
                Assert.That(hasSceneObjectSelectionMethod, Is.Not.Null);
                Assert.That((int)getSceneObjectSelectionCountMethod.Invoke(null, null), Is.EqualTo(0));
                Assert.That((bool)hasSceneObjectSelectionMethod.Invoke(null, null), Is.False);
            }
            finally
            {
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }

        [Test]
        public void HasValidBatchSelection_GivenSceneAndPrefabAssetSelection_ReturnsTrue()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string scenePath = scene.SaveToDisk();
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo hasValidBatchSelectionMethod = menuValidatorType?.GetMethod("HasValidBatchSelection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Select assets
                Selection.objects = new Object[]
                {
                    sceneAsset,
                    prefabAsset
                };

                // Assert
                Assert.That(sceneAsset, Is.Not.Null);
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(hasValidBatchSelectionMethod, Is.Not.Null);
                Assert.That((bool)hasValidBatchSelectionMethod.Invoke(null, null), Is.True);
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
        public void HasValidBatchSelection_GivenFolderContainingPrefabAsset_ReturnsTrue()
        {
            // Set up folder
            string folderPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/MenuValidatorFolder");
            string parentFolderPath = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            GameObject prefabRoot = new("Prefab Root");
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo hasValidBatchSelectionMethod = menuValidatorType?.GetMethod("HasValidBatchSelection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Create asset folder and prefab
                AssetDatabase.CreateFolder(parentFolderPath, folderName);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, $"{folderPath}/MenuValidator.prefab");
                Object folderAsset = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                Selection.objects = new[] { folderAsset };

                // Assert
                Assert.That(folderAsset, Is.Not.Null);
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(hasValidBatchSelectionMethod, Is.Not.Null);
                Assert.That((bool)hasValidBatchSelectionMethod.Invoke(null, null), Is.True);
            }
            finally
            {
                Object.DestroyImmediate(prefabRoot);
                AssetDatabase.DeleteAsset(folderPath);
            }
        }

        [Test]
        public void HasValidBatchSelection_GivenInvalidAssetSelection_ReturnsFalse()
        {
            // Set up asset
            AssetDependency dependency = ScriptableObject.CreateInstance<AssetDependency>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/MenuValidatorTests.asset");
            AssetDatabase.CreateAsset(dependency, assetPath);
            Type menuValidatorType = typeof(SelectSameContextMenus).Assembly.GetType("Plugins.Saneject.Editor.Menus.MenuValidator");
            MethodInfo hasValidBatchSelectionMethod = menuValidatorType?.GetMethod("HasValidBatchSelection", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                // Select asset
                Selection.objects = new Object[] { dependency };

                // Assert
                Assert.That(menuValidatorType, Is.Not.Null);
                Assert.That(hasValidBatchSelectionMethod, Is.Not.Null);
                Assert.That((bool)hasValidBatchSelectionMethod.Invoke(null, null), Is.False);
            }
            finally
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
        }
    }
}
