using System;
using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.BatchInjection.Core;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Tests.Runtime.BatchInjection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Editor.BatchInjection
{
    public class BatchInjectionTests
    {
        [Test]
        public void SceneBatchInjectionHasNoMissingDependencies()
        {
            AssetData[] sceneData =
            {
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 1.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 2.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 3.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 4.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 5.unity"))
            };

            ResetScenes(sceneData);
            BatchInjector.BatchInjectScenes(sceneData, false);
            AssertSceneDependencies(sceneData);
        }

        [Test]
        public void PrefabBatchInjectionHasNoMissingDependencies()
        {
            AssetData[] prefabData =
            {
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 1.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 2.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 3.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 4.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 5.prefab"))
            };

            ResetPrefabs(prefabData);
            BatchInjector.BatchInjectPrefabs(prefabData, false);
            AssertPrefabDependencies(prefabData);
        }

        [Test]
        public void SceneAndPrefabInjectionHasNoMissingDependencies()
        {
            AssetData[] sceneData =
            {
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 1.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 2.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 3.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 4.unity")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/TestScene 5.unity"))
            };

            AssetData[] prefabData =
            {
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 1.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 2.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 3.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 4.prefab")),
                new(AssetDatabase.AssetPathToGUID("Assets/Tests/Runtime/BatchInjection/Prefab 5.prefab"))
            };

            ResetScenes(sceneData);
            ResetPrefabs(prefabData);
            BatchInjector.BatchInjectAllScenesAndPrefabs(prefabData, sceneData);
            AssertSceneDependencies(sceneData);
            AssertPrefabDependencies(prefabData);
        }

        private static void ResetScenes(AssetData[] assetDatas)
        {
            bool foundTestRequester = false;

            foreach (AssetData data in assetDatas)
            {
                Scene scene = EditorSceneManager.OpenScene(data.Path);

                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                    if (rootGameObject.TryGetComponent(out TestRequester testRequester))
                    {
                        testRequester.dependency = null;
                        foundTestRequester = true;
                        Assert.Null(testRequester.dependency);
                        break;
                    }
            }

            Assert.IsTrue(foundTestRequester);
        }

        private static void AssertSceneDependencies(AssetData[] assetDatas)
        {
            foreach (AssetData data in assetDatas)
            {
                Scene scene = EditorSceneManager.OpenScene(data.Path);

                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                    if (rootGameObject.TryGetComponent(out TestRequester testRequester))
                    {
                        Assert.NotNull(testRequester);
                        Assert.NotNull(testRequester.dependency);
                    }
            }
        }

        private static void AssertPrefabDependencies(AssetData[] assetDatas)
        {
            foreach (GameObject gameObject in assetDatas.Select(data => data.Asset as GameObject))
            {
                Assert.NotNull(gameObject);
                Assert.NotNull(gameObject.GetComponent<TestRequester>().dependency);
            }
        }

        private static void ResetPrefabs(AssetData[] assetDatas)
        {
            bool foundTestRequester = false;

            foreach (GameObject gameObject in assetDatas.Select(data => data.Asset as GameObject))
            {
                if (gameObject == null)
                    throw new NullReferenceException();

                if (gameObject.TryGetComponent(out TestRequester testRequester))
                {
                    testRequester.dependency = null;
                    foundTestRequester = true;
                    Assert.Null(testRequester.dependency);
                }
            }

            Assert.IsTrue(foundTestRequester);
        }
    }
}