using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Core;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectMenuItems
    {
        private const int MenuRoot = -10000;
        private const int GroupCurrent = MenuRoot + 0;
        private const int GroupAll = MenuRoot + 20;
        private const int GroupSelected = MenuRoot + 40;
        private const int GroupBatch = MenuRoot + 60;

        #region Menu item methods

        [MenuItem("GameObject/Saneject/Inject Current Scene", false, GroupCurrent + 1),
         MenuItem("Saneject/Inject/Current Scene", false, GroupCurrent + 1)]
        private static void InjectCurrentScene()
        {
            GameObject[] startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Current Prefab", false, GroupCurrent + 2),
         MenuItem("Saneject/Inject/Current Prefab", false, GroupCurrent + 2)]
        private static void InjectCurrentPrefab()
        {
            GameObject[] startObjects = { PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot };

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabAsset);
        }

        [MenuItem("GameObject/Saneject/Inject All Scene Objects", false, GroupAll + 1),
         MenuItem("Saneject/Inject/All Scene Objects", false, GroupAll + 1)]
        private static void InjectAllSceneObjects()
        {
            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.SceneObject);
        }

        [MenuItem("GameObject/Saneject/Inject All Scene Prefab Instances", false, GroupAll + 2),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", false, GroupAll + 2)]
        private static void InjectAllScenePrefabInstances()
        {
            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabInstance);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchy (All Contexts)", false, GroupSelected + 1),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (All Contexts)", false, GroupSelected + 1)]
        private static void InjectSelectedSceneHierarchyAllContexts()
        {
            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.SceneObject);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchy (Start Object Contexts Only)", false, GroupSelected + 2),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", false, GroupSelected + 2)]
        private static void InjectSelectedSceneHierarchyStartObjectContextOnly()
        {
            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.SameAsStartObjects);
        }

        [MenuItem("Assets/Saneject/Inject Selected Assets", false, GroupBatch + 1),
         MenuItem("Saneject/Inject/Selected Assets", false, GroupBatch + 1)]
        private static void InjectSelectedAssets()
        {
            BatchItem[] batchItems = Selection.GetFiltered<Object>(SelectionMode.DeepAssets)
                .CreateBatchItemsFromObjects(ContextWalkFilter.All)
                .ToArray();

            if (batchItems.Length > 1)
            {
                int sceneCount = batchItems.OfType<SceneBatchItem>().Count();
                int prefabCount = batchItems.OfType<PrefabBatchItem>().Count();

                if (!DialogUtility.InjectionMenus.UserAcceptedBatchInjection(sceneCount, prefabCount))
                    return;
            }

            InjectionRunner.RunBatch
            (
                batchItems
            );
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Inject Current Scene", true),
         MenuItem("Saneject/Inject/Current Scene", true)]
        private static bool InjectCurrentScene_Validate()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Current Prefab", true),
         MenuItem("Saneject/Inject/Current Prefab", true)]
        private static bool InjectCurrentPrefab_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        [MenuItem("GameObject/Saneject/Inject All Scene Objects", true),
         MenuItem("Saneject/Inject/All Scene Objects", true)]
        private static bool InjectAllSceneObjects_Validate()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject All Scene Prefab Instances", true, -10049),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", true, -10049)]
        private static bool InjectAllScenePrefabInstances_Validate()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchy (All Contexts)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (All Contexts)", true)]
        private static bool InjectSelectedSceneHierarchyAllContexts_Validate()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchy (Start Object Contexts Only)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", true)]
        private static bool InjectSelectedSceneHierarchyStartObjectContextOnly_Validate()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("Assets/Saneject/Inject Selected Assets", true),
         MenuItem("Saneject/Inject/Selected Assets", true)]
        private static bool InjectSelectedAssets_Validate()
        {
            return Selection
                .GetFiltered<Object>(SelectionMode.DeepAssets)
                .Any(x => x is GameObject or SceneAsset);
        }

        #endregion
    }
}