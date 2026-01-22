using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 1;

        private const int Priority_Group_Current = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_InjectCurrentScene = Priority_Group_Current + 1;
        private const int Priority_Item_InjectCurrentPrefab = Priority_Group_Current + 2;

        private const int Priority_Group_All = Priority_Base + MenuPriority.Group * 1;
        private const int Priority_Item_InjectAllSceneObjects = Priority_Group_All + 1;
        private const int Priority_Item_InjectAllScenePrefabInstances = Priority_Group_All + 2;

        private const int Priority_Group_Selected = Priority_Base + MenuPriority.Group * 2;
        private const int Priority_Item_InjectSelectedSceneHierarchyAllContexts = Priority_Group_Selected + 1;
        private const int Priority_Item_InjectSelectedSceneHierarchyStartObjectContextOnly = Priority_Group_Selected + 2;

        private const int Priority_Group_Batch = Priority_Base + MenuPriority.Group * 3;
        private const int Priority_Item_InjectSelectedAssets = Priority_Group_Batch + 1;

        #endregion

        #region Menu item methods

        [MenuItem("GameObject/Saneject/Inject/Current Scene", false, Priority_Item_InjectCurrentScene),
         MenuItem("Saneject/Inject/Current Scene", false, Priority_Item_InjectCurrentScene)]
        private static void InjectCurrentScene()
        {
            GameObject[] startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab", false, Priority_Item_InjectCurrentPrefab),
         MenuItem("Saneject/Inject/Current Prefab", false, Priority_Item_InjectCurrentPrefab)]
        private static void InjectCurrentPrefab()
        {
            GameObject[] startObjects = { PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot };

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabAsset);
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Objects", false, Priority_Item_InjectAllSceneObjects),
         MenuItem("Saneject/Inject/All Scene Objects", false, Priority_Item_InjectAllSceneObjects)]
        private static void InjectAllSceneObjects()
        {
            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.SceneObject);
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Prefab Instances", false, Priority_Item_InjectAllScenePrefabInstances),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", false, Priority_Item_InjectAllScenePrefabInstances)]
        private static void InjectAllScenePrefabInstances()
        {
            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabInstance);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchy (All Contexts)", false, Priority_Item_InjectSelectedSceneHierarchyAllContexts),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (All Contexts)", false, Priority_Item_InjectSelectedSceneHierarchyAllContexts)]
        private static void InjectSelectedSceneHierarchyAllContexts()
        {
            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", false, Priority_Item_InjectSelectedSceneHierarchyStartObjectContextOnly),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", false, Priority_Item_InjectSelectedSceneHierarchyStartObjectContextOnly)]
        private static void InjectSelectedSceneHierarchyStartObjectContextOnly()
        {
            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.SameAsStartObjects);
        }

        [MenuItem("Assets/Saneject/Inject/Selected Assets", false, Priority_Item_InjectSelectedAssets),
         MenuItem("Saneject/Inject/Selected Assets", false, Priority_Item_InjectSelectedAssets)]
        private static void InjectSelectedAssets()
        {
            BatchItem[] batchItems = Selection.GetFiltered<Object>(SelectionMode.DeepAssets)
                .CreateBatchItemsFromObjects(ContextWalkFilter.All)
                .ToArray();

            if (batchItems.Length > 1)
            {
                int sceneCount = batchItems.OfType<SceneBatchItem>().Count();
                int prefabCount = batchItems.OfType<PrefabBatchItem>().Count();

                if (!DisplayDialog.BatchInjection.UserConfirmed(sceneCount, prefabCount))
                    return;
            }

            InjectionRunner.RunBatch
            (
                batchItems
            );
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Inject/Current Scene", true),
         MenuItem("Saneject/Inject/Current Scene", true)]
        private static bool Validate_InjectCurrentScene()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab", true),
         MenuItem("Saneject/Inject/Current Prefab", true)]
        private static bool Validate_InjectCurrentPrefab()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Objects", true),
         MenuItem("Saneject/Inject/All Scene Objects", true)]
        private static bool Validate_InjectAllSceneObjects()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Prefab Instances", true),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", true)]
        private static bool Validate_InjectAllScenePrefabInstances()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchy (All Contexts)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (All Contexts)", true)]
        private static bool Validate_InjectSelectedSceneHierarchyAllContexts()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchy (Start Object Contexts Only)", true)]
        private static bool Validate_InjectSelectedSceneHierarchyStartObjectContextOnly()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("Assets/Saneject/Inject/Selected Assets", true),
         MenuItem("Saneject/Inject/Selected Assets", true)]
        private static bool Validate_InjectSelectedAssets()
        {
            return Selection
                .GetFiltered<Object>(SelectionMode.DeepAssets)
                .Any(x => x is GameObject or SceneAsset);
        }

        #endregion
    }
}