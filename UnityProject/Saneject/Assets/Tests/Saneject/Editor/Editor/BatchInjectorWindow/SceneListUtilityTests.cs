using System.IO;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities;
using Tests.Saneject.Fixtures.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class SceneListUtilityTests
    {
        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void AddOpenScenes_GivenSavedOpenScenes_AddsSortedScenesAndMarksDirty()
        {
            // Set up folder
            string folderPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/SceneListUtility");
            string parentFolderPath = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parentFolderPath, folderName);
            TestScene firstScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string firstScenePath = $"{folderPath}/Scene 10.unity";
            EditorSceneManager.SaveScene(firstScene.Scene, firstScenePath);
            TestScene secondScene = TestScene.Create(roots: 1, width: 1, depth: 1, newSceneMode: NewSceneMode.Additive);
            string secondScenePath = $"{folderPath}/Scene 2.unity";
            EditorSceneManager.SaveScene(secondScene.Scene, secondScenePath);
            BatchInjectorData batchInjectorData = new();

            try
            {
                // Add scenes
                SceneListUtility.AddOpenScenes(batchInjectorData);

                // Assert
                Assert.That(batchInjectorData.IsDirty, Is.True);
                Assert.That(batchInjectorData.SceneList.TotalCount, Is.EqualTo(2));
                CollectionAssert.AreEqual(new[]
                {
                    secondScenePath,
                    firstScenePath
                }, Enumerable.Range(0, batchInjectorData.SceneList.TotalCount)
                    .Select(index => batchInjectorData.SceneList.GetElementAt(index).GetAssetPath())
                    .ToArray());
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                AssetDatabase.DeleteAsset(folderPath);
            }
        }

        [Test]
        public void AddOpenScenes_GivenUnsavedOpenScene_IgnoresUnsavedScene()
        {
            // Set up folder
            string folderPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/UnsavedSceneListUtility");
            string parentFolderPath = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parentFolderPath, folderName);
            TestScene savedScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            string savedScenePath = $"{folderPath}/Saved Scene.unity";
            EditorSceneManager.SaveScene(savedScene.Scene, savedScenePath);
            TestScene.Create(roots: 1, width: 1, depth: 1, newSceneMode: NewSceneMode.Additive);
            BatchInjectorData batchInjectorData = new();

            try
            {
                // Add scenes
                SceneListUtility.AddOpenScenes(batchInjectorData);

                // Assert
                Assert.That(batchInjectorData.IsDirty, Is.True);
                Assert.That(batchInjectorData.SceneList.TotalCount, Is.EqualTo(1));
                CollectionAssert.AreEqual(new[]
                {
                    savedScenePath
                }, new[]
                {
                    batchInjectorData.SceneList.GetElementAt(0).GetAssetPath()
                });
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                AssetDatabase.DeleteAsset(folderPath);
            }
        }
    }
}
