using System;
using NUnit.Framework;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Settings;
using Tests.Saneject.Fixtures.Scripts;
using Tests.Saneject.Fixtures.Scripts.Dependencies;
using Tests.Saneject.Fixtures.Scripts.InjectionTargets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Editor.Editor.Utilities
{
    public class InjectionUtilityTests
    {
        private bool originalAskBeforeInjectScene;
        private bool originalAskBeforeInjectSelectedSceneHierarchies;
        private bool originalAskBeforeInjectPrefabAsset;
        private bool originalAskBeforeBatchInject;

        [SetUp]
        public void SetUp()
        {
            originalAskBeforeInjectScene = UserSettings.AskBeforeInjectScene;
            originalAskBeforeInjectSelectedSceneHierarchies = UserSettings.AskBeforeInjectSelectedSceneHierarchies;
            originalAskBeforeInjectPrefabAsset = UserSettings.AskBeforeInjectPrefabAsset;
            originalAskBeforeBatchInject = UserSettings.AskBeforeBatchInject;

            UserSettings.AskBeforeInjectScene = false;
            UserSettings.AskBeforeInjectSelectedSceneHierarchies = false;
            UserSettings.AskBeforeInjectPrefabAsset = false;
            UserSettings.AskBeforeBatchInject = false;
        }

        [TearDown]
        public void TearDown()
        {
            UserSettings.AskBeforeInjectScene = originalAskBeforeInjectScene;
            UserSettings.AskBeforeInjectSelectedSceneHierarchies = originalAskBeforeInjectSelectedSceneHierarchies;
            UserSettings.AskBeforeInjectPrefabAsset = originalAskBeforeInjectPrefabAsset;
            UserSettings.AskBeforeBatchInject = originalAskBeforeBatchInject;

            Selection.objects = Array.Empty<Object>();

            TestPrefabAsset.CloseStage();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void BatchInjectSelectedAssets_GivenSelectedSceneAndPrefabAssets_InjectsSelectedAssets()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 1, width: 1, depth: 1);
            scene.Add<BatchInjectionScope>("Root 1");
            scene.Add<SingleConcreteComponentTarget>("Root 1");
            scene.Add<ComponentDependency>("Root 1");
            string scenePath = scene.SaveToDisk();
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            prefab.Add<BatchInjectionScope>("Prefab Root");
            prefab.Add<SingleConcreteComponentTarget>("Prefab Root");
            prefab.Add<ComponentDependency>("Prefab Root");
            PrefabUtility.SaveAsPrefabAsset(prefab.Root, prefab.AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);

            try
            {
                // Select assets
                Selection.objects = new Object[]
                {
                    sceneAsset,
                    prefabAsset
                };

                // Inject
                InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.AllContexts);

                // Reopen scene
                Scene injectedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                GameObject injectedSceneRoot = injectedScene.GetRootGameObjects()[0];
                ComponentDependency sceneDependency = injectedSceneRoot.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget sceneTarget = injectedSceneRoot.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(sceneAsset, Is.Not.Null);
                Assert.That(prefabAsset, Is.Not.Null);
                Assert.That(sceneDependency, Is.Not.Null);
                Assert.That(sceneTarget, Is.Not.Null);
                Assert.That(sceneTarget.dependency, Is.EqualTo(sceneDependency));

                // Load prefab asset
                GameObject injectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.AssetPath);

                // Assert
                Assert.That(injectedPrefab, Is.Not.Null);

                ComponentDependency prefabDependency = injectedPrefab.GetComponent<ComponentDependency>();
                SingleConcreteComponentTarget prefabTarget = injectedPrefab.GetComponent<SingleConcreteComponentTarget>();

                // Assert
                Assert.That(prefabDependency, Is.Not.Null);
                Assert.That(prefabTarget, Is.Not.Null);
                Assert.That(prefabTarget.dependency, Is.EqualTo(prefabDependency));
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
        public void InjectCurrentScene_GivenMultipleOpenScenes_InjectsActiveScene()
        {
            // Set up first scene
            TestScene firstScene = TestScene.Create(roots: 1, width: 1, depth: 1);
            firstScene.SaveToDisk();
            TestScope firstScope = firstScene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget firstTarget = firstScene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency firstDependency = firstScene.Add<ComponentDependency>("Root 1");

            // Set up second scene
            TestScene secondScene = TestScene.Create(roots: 1, width: 1, depth: 1, newSceneMode: NewSceneMode.Additive);
            secondScene.SaveToDisk();
            TestScope secondScope = secondScene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget secondTarget = secondScene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency secondDependency = secondScene.Add<ComponentDependency>("Root 1");

            try
            {
                // Bind
                firstScope.BindComponent<ComponentDependency>().FromSelf();
                secondScope.BindComponent<ComponentDependency>().FromSelf();

                // Assert active scene
                Assert.That(SceneManager.GetActiveScene().handle, Is.EqualTo(secondScene.Scene.handle));

                // Inject
                InjectionUtility.InjectCurrentScene(ContextWalkFilter.SceneObjects);

                // Assert
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(firstTarget.dependency, Is.Null);
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
        public void InjectSelectedSceneHierarchies_GivenSceneSelection_InjectsSelectedHierarchies()
        {
            // Set up scene
            TestScene scene = TestScene.Create(roots: 2, width: 1, depth: 1);
            TestScope firstScope = scene.Add<TestScope>("Root 1");
            SingleConcreteComponentTarget firstTarget = scene.Add<SingleConcreteComponentTarget>("Root 1");
            ComponentDependency firstDependency = scene.Add<ComponentDependency>("Root 1");
            TestScope secondScope = scene.Add<TestScope>("Root 2");
            SingleConcreteComponentTarget secondTarget = scene.Add<SingleConcreteComponentTarget>("Root 2");
            ComponentDependency secondDependency = scene.Add<ComponentDependency>("Root 2");

            // Bind
            firstScope.BindComponent<ComponentDependency>().FromSelf();
            secondScope.BindComponent<ComponentDependency>().FromSelf();

            // Select hierarchy
            Selection.objects = new Object[] { scene.GetTransform("Root 1").gameObject };

            // Inject
            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SceneObjects);

            // Assert
            Assert.That(firstDependency, Is.Not.Null);
            Assert.That(secondDependency, Is.Not.Null);
            Assert.That(firstTarget.dependency, Is.EqualTo(firstDependency));
            Assert.That(secondTarget.dependency, Is.Null);
        }

        [Test]
        public void InjectCurrentPrefabAsset_GivenPrefabAssetStage_InjectsCurrentPrefabAsset()
        {
            // Set up host prefab asset
            TestPrefabAsset hostPrefab = TestPrefabAsset.Create("Prefab Root", width: 1, depth: 1);
            TestPrefabInstance hostStage = hostPrefab.OpenStage();
            TestScope hostScope = hostStage.Add<TestScope>("Prefab Root");
            SingleConcreteComponentTarget hostTarget = hostStage.Add<SingleConcreteComponentTarget>("Prefab Root");
            ComponentDependency hostDependency = hostStage.Add<ComponentDependency>("Prefab Root");

            // Set up nested prefab instance
            TestPrefabAsset nestedPrefab = TestPrefabAsset.Create("Nested Root", width: 1, depth: 1);
            TestPrefabInstance nestedInstance = nestedPrefab.Instantiate(hostStage.GetTransform("Prefab Root"), "Nested Prefab");
            TestScope nestedScope = nestedInstance.Add<TestScope>("Nested Root");
            SingleConcreteComponentTarget nestedTarget = nestedInstance.Add<SingleConcreteComponentTarget>("Nested Root");
            ComponentDependency nestedDependency = nestedInstance.Add<ComponentDependency>("Nested Root");

            try
            {
                // Bind
                hostScope.BindComponent<ComponentDependency>().FromSelf();
                nestedScope.BindComponent<ComponentDependency>().FromSelf();

                // Inject
                InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabAssetObjects);

                // Assert
                Assert.That(hostDependency, Is.Not.Null);
                Assert.That(nestedDependency, Is.Not.Null);
                Assert.That(hostTarget.dependency, Is.EqualTo(hostDependency));
                Assert.That(nestedTarget.dependency, Is.Null);
            }
            finally
            {
                nestedInstance.Destroy();
                TestPrefabAsset.CloseStage();
                hostPrefab.Destroy();
                hostPrefab.DeleteAsset();
                nestedPrefab.Destroy();
                nestedPrefab.DeleteAsset();
            }
        }

        [Test]
        public void InjectPrefabAssetSelection_GivenPrefabAssetSelection_InjectsPrefabAssetObjectsFromSelectedRoot()
        {
            // Set up prefab asset
            TestPrefabAsset prefab = TestPrefabAsset.Create("Prefab Root", width: 2, depth: 2);
            TestPrefabInstance prefabStage = prefab.OpenStage();
            TestScope firstScope = prefabStage.Add<TestScope>("Prefab Root/Child 1");
            SingleConcreteComponentTarget firstTarget = prefabStage.Add<SingleConcreteComponentTarget>("Prefab Root/Child 1");
            ComponentDependency firstDependency = prefabStage.Add<ComponentDependency>("Prefab Root/Child 1");
            TestScope secondScope = prefabStage.Add<TestScope>("Prefab Root/Child 2");
            SingleConcreteComponentTarget secondTarget = prefabStage.Add<SingleConcreteComponentTarget>("Prefab Root/Child 2");
            ComponentDependency secondDependency = prefabStage.Add<ComponentDependency>("Prefab Root/Child 2");

            try
            {
                // Bind
                firstScope.BindComponent<ComponentDependency>().FromSelf();
                secondScope.BindComponent<ComponentDependency>().FromSelf();

                // Select hierarchy
                Selection.objects = new Object[] { prefabStage.GetTransform("Prefab Root/Child 1").gameObject };

                // Inject
                InjectionUtility.InjectPrefabAssetSelection(ContextWalkFilter.PrefabAssetObjects);

                // Assert
                Assert.That(firstDependency, Is.Not.Null);
                Assert.That(secondDependency, Is.Not.Null);
                Assert.That(firstTarget.dependency, Is.EqualTo(firstDependency));
                Assert.That(secondTarget.dependency, Is.EqualTo(secondDependency));
            }
            finally
            {
                TestPrefabAsset.CloseStage();
                prefab.Destroy();
                prefab.DeleteAsset();
            }
        }
    }
}
