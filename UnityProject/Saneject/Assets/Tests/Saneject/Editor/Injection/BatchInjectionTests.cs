using NUnit.Framework;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Pipeline;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Saneject.Editor.Injection
{
    public class BatchInjectionTests
    {
        [Test]
        public void RunBatch_GivenSceneBatchItems_InjectsScenes()
        {
            // Set up first scene
            TestScene firstScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            firstScene.Add<BatchInjectionScope>("Root 1");
            firstScene.Add<SingleConcreteComponentTarget>("Root 1");
            firstScene.Add<ComponentDependency>("Root 1");
            string firstScenePath = firstScene.SaveToDisk();
            SceneBatchItem firstBatchItem = new(firstScenePath, ContextWalkFilter.SceneObjects);

            // Set up second scene
            TestScene secondScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            secondScene.Add<BatchInjectionScope>("Root 1");
            secondScene.Add<SingleConcreteComponentTarget>("Root 1");
            secondScene.Add<ComponentDependency>("Root 1");
            string secondScenePath = secondScene.SaveToDisk();
            SceneBatchItem secondBatchItem = new(secondScenePath, ContextWalkFilter.SceneObjects);

            try
            {
                // Batch inject
                InjectionRunner.RunBatch
                (
                    new BatchItem[]
                    {
                        firstBatchItem,
                        secondBatchItem
                    }
                );

                // Reopen first scene
                Scene injectedFirstScene = EditorSceneManager.OpenScene(firstScenePath, OpenSceneMode.Single);
                GameObject injectedFirstRoot = injectedFirstScene.GetRootGameObjects()[0];
                ComponentDependency firstDependency = injectedFirstRoot.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget firstTarget = injectedFirstRoot.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(firstBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(firstTarget, Is.Not.Null);
                Assert.That(firstTarget.dependency, Is.EqualTo(firstDependency));

                // Reopen second scene
                Scene injectedSecondScene = EditorSceneManager.OpenScene(secondScenePath, OpenSceneMode.Single);
                GameObject injectedSecondRoot = injectedSecondScene.GetRootGameObjects()[0];
                ComponentDependency secondDependency = injectedSecondRoot.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget secondTarget = injectedSecondRoot.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(secondBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(secondTarget, Is.Not.Null);
                Assert.That(secondTarget.dependency, Is.EqualTo(secondDependency));
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                firstScene.DeleteFromDisk();
                secondScene.DeleteFromDisk();
            }
        }

        [Test]
        public void RunBatch_GivenPrefabBatchItems_InjectsPrefabs()
        {
            // Set up first prefab
            TestPrefabAsset firstPrefab = TestPrefabAsset.Create("Prefab Root A", width: 1, depth: 1);
            firstPrefab.Add<BatchInjectionScope>("Prefab Root A");
            firstPrefab.Add<SingleConcreteComponentTarget>("Prefab Root A");
            firstPrefab.Add<ComponentDependency>("Prefab Root A");
            TestPrefabInstance firstPrefabStage = firstPrefab.OpenStage();
            PrefabBatchItem firstBatchItem = new(firstPrefab.AssetPath, ContextWalkFilter.PrefabAssetObjects);

            // Set up second prefab
            TestPrefabAsset secondPrefab = TestPrefabAsset.Create("Prefab Root B", width: 1, depth: 1);
            secondPrefab.Add<BatchInjectionScope>("Prefab Root B");
            secondPrefab.Add<SingleConcreteComponentTarget>("Prefab Root B");
            secondPrefab.Add<ComponentDependency>("Prefab Root B");
            TestPrefabInstance secondPrefabStage = secondPrefab.OpenStage();
            PrefabBatchItem secondBatchItem = new(secondPrefab.AssetPath, ContextWalkFilter.PrefabAssetObjects);

            try
            {
                // Close stages
                Assert.That(firstPrefabStage, Is.Not.Null);
                Assert.That(secondPrefabStage, Is.Not.Null);
                TestPrefabAsset.CloseStage();

                // Batch inject
                InjectionRunner.RunBatch
                (
                    new BatchItem[]
                    {
                        firstBatchItem,
                        secondBatchItem
                    }
                );

                // Load first prefab
                GameObject injectedFirstPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(firstPrefab.AssetPath);

                // Assert
                Assert.That(injectedFirstPrefab, Is.Not.Null);

                ComponentDependency firstDependency = injectedFirstPrefab.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget firstTarget = injectedFirstPrefab.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(firstBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(firstTarget, Is.Not.Null);
                Assert.That(firstTarget.dependency, Is.EqualTo(firstDependency));

                // Load second prefab
                GameObject injectedSecondPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(secondPrefab.AssetPath);

                // Assert
                Assert.That(injectedSecondPrefab, Is.Not.Null);

                ComponentDependency secondDependency = injectedSecondPrefab.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget secondTarget = injectedSecondPrefab.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(secondBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(secondTarget, Is.Not.Null);
                Assert.That(secondTarget.dependency, Is.EqualTo(secondDependency));
            }
            finally
            {
                TestPrefabAsset.CloseStage();

                firstPrefab.Destroy();
                firstPrefab.DeleteAsset();

                secondPrefab.Destroy();
                secondPrefab.DeleteAsset();
            }
        }

        [Test]
        public void RunBatch_GivenSceneAndPrefabBatchItems_InjectsScenesAndPrefabs()
        {
            // Set up first scene
            TestScene firstScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            firstScene.Add<BatchInjectionScope>("Root 1");
            firstScene.Add<SingleConcreteComponentTarget>("Root 1");
            firstScene.Add<ComponentDependency>("Root 1");
            string firstScenePath = firstScene.SaveToDisk();
            SceneBatchItem firstSceneBatchItem = new(firstScenePath, ContextWalkFilter.SceneObjects);

            // Set up second scene
            TestScene secondScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            secondScene.Add<BatchInjectionScope>("Root 1");
            secondScene.Add<SingleConcreteComponentTarget>("Root 1");
            secondScene.Add<ComponentDependency>("Root 1");
            string secondScenePath = secondScene.SaveToDisk();
            SceneBatchItem secondSceneBatchItem = new(secondScenePath, ContextWalkFilter.SceneObjects);

            // Set up first prefab
            TestPrefabAsset firstPrefab = TestPrefabAsset.Create("Prefab Root A", width: 1, depth: 1);
            firstPrefab.Add<BatchInjectionScope>("Prefab Root A");
            firstPrefab.Add<SingleConcreteComponentTarget>("Prefab Root A");
            firstPrefab.Add<ComponentDependency>("Prefab Root A");
            TestPrefabInstance firstPrefabStage = firstPrefab.OpenStage();
            Assert.That(firstPrefabStage, Is.Not.Null);
            TestPrefabAsset.CloseStage();
            PrefabBatchItem firstPrefabBatchItem = new(firstPrefab.AssetPath, ContextWalkFilter.PrefabAssetObjects);

            // Set up second prefab
            TestPrefabAsset secondPrefab = TestPrefabAsset.Create("Prefab Root B", width: 1, depth: 1);
            secondPrefab.Add<BatchInjectionScope>("Prefab Root B");
            secondPrefab.Add<SingleConcreteComponentTarget>("Prefab Root B");
            secondPrefab.Add<ComponentDependency>("Prefab Root B");
            TestPrefabInstance secondPrefabStage = secondPrefab.OpenStage();
            Assert.That(secondPrefabStage, Is.Not.Null);
            TestPrefabAsset.CloseStage();
            PrefabBatchItem secondPrefabBatchItem = new(secondPrefab.AssetPath, ContextWalkFilter.PrefabAssetObjects);

            try
            {
                // Batch inject
                InjectionRunner.RunBatch
                (
                    new BatchItem[]
                    {
                        firstSceneBatchItem,
                        firstPrefabBatchItem,
                        secondSceneBatchItem,
                        secondPrefabBatchItem
                    }
                );

                // Reopen first scene
                Scene injectedFirstScene = EditorSceneManager.OpenScene(firstScenePath, OpenSceneMode.Single);
                GameObject injectedFirstSceneRoot = injectedFirstScene.GetRootGameObjects()[0];
                ComponentDependency firstSceneDependency = injectedFirstSceneRoot.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget firstSceneTarget = injectedFirstSceneRoot.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(firstSceneDependency, Is.Not.Null);
                Assert.That(firstSceneTarget, Is.Not.Null);
                Assert.That(firstSceneTarget.dependency, Is.EqualTo(firstSceneDependency));

                // Reopen second scene
                Scene injectedSecondScene = EditorSceneManager.OpenScene(secondScenePath, OpenSceneMode.Single);
                GameObject injectedSecondSceneRoot = injectedSecondScene.GetRootGameObjects()[0];
                ComponentDependency secondSceneDependency = injectedSecondSceneRoot.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget secondSceneTarget = injectedSecondSceneRoot.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(secondSceneDependency, Is.Not.Null);
                Assert.That(secondSceneTarget, Is.Not.Null);
                Assert.That(secondSceneTarget.dependency, Is.EqualTo(secondSceneDependency));

                // Load first prefab
                GameObject injectedFirstPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(firstPrefab.AssetPath);

                // Assert
                Assert.That(injectedFirstPrefab, Is.Not.Null);

                ComponentDependency firstPrefabDependency = injectedFirstPrefab.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget firstPrefabTarget = injectedFirstPrefab.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(firstPrefabDependency, Is.Not.Null);
                Assert.That(firstPrefabTarget, Is.Not.Null);
                Assert.That(firstPrefabTarget.dependency, Is.EqualTo(firstPrefabDependency));

                // Load second prefab
                GameObject injectedSecondPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(secondPrefab.AssetPath);

                // Assert
                Assert.That(injectedSecondPrefab, Is.Not.Null);

                ComponentDependency secondPrefabDependency = injectedSecondPrefab.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget secondPrefabTarget = injectedSecondPrefab.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(secondPrefabDependency, Is.Not.Null);
                Assert.That(secondPrefabTarget, Is.Not.Null);
                Assert.That(secondPrefabTarget.dependency, Is.EqualTo(secondPrefabDependency));
                Assert.That(firstSceneBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(secondSceneBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(firstPrefabBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
                Assert.That(secondPrefabBatchItem.Status, Is.EqualTo(InjectionStatus.Success));
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                firstScene.DeleteFromDisk();
                secondScene.DeleteFromDisk();

                firstPrefab.Destroy();
                firstPrefab.DeleteAsset();

                secondPrefab.Destroy();
                secondPrefab.DeleteAsset();
            }
        }
    }
}
