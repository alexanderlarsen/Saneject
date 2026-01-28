using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Context;
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

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 1 + 1;

        private const int Priority_Group_Current = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_Inject_CurrentScene = Priority_Group_Current + 1;
        private const int Priority_Item_Inject_CurrentPrefab = Priority_Group_Current + 2;

        private const int Priority_Group_All = Priority_Base + MenuPriority.Group * 1;
        private const int Priority_Item_Inject_AllSceneObjects = Priority_Group_All + 1;
        private const int Priority_Item_Inject_AllScenePrefabInstances = Priority_Group_All + 2;

        private const int Priority_Group_Selected = Priority_Base + MenuPriority.Group * 2;
        private const int Priority_Item_Confirm_Inject_SelectedSceneHierarchy_AllContexts = Priority_Group_Selected + 1;
        private const int Priority_Item_Inject_SelectedSceneHierarchies_SelectedObjectContextsOnly = Priority_Group_Selected + 2;

        #endregion

        #region Menu item methods

        [MenuItem("GameObject/Saneject/Inject/Current Scene", false, Priority_Item_Inject_CurrentScene),
         MenuItem("Saneject/Inject/Current Scene", false, Priority_Item_Inject_CurrentScene)]
        private static void Inject_CurrentScene(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_CurrentScene())
                return;

            GameObject[] startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab", false, Priority_Item_Inject_CurrentPrefab),
         MenuItem("Saneject/Inject/Current Prefab", false, Priority_Item_Inject_CurrentPrefab)]
        private static void Inject_CurrentPrefab(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_CurrentPrefab())
                return;

            GameObject[] startObjects =
            {
                PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
            };

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabAsset);
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Objects", false, Priority_Item_Inject_AllSceneObjects),
         MenuItem("Saneject/Inject/All Scene Objects", false, Priority_Item_Inject_AllSceneObjects)]
        private static void Inject_AllSceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_AllSceneObjects())
                return;

            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.SceneObject);
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Prefab Instances", false, Priority_Item_Inject_AllScenePrefabInstances),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", false, Priority_Item_Inject_AllScenePrefabInstances)]
        private static void Inject_AllScenePrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_AllScenePrefabInstances())
                return;

            IEnumerable<GameObject> startObjects =
                SceneManager
                    .GetActiveScene()
                    .GetRootGameObjects();

            InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabInstance);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (All Contexts)", false, Priority_Item_Confirm_Inject_SelectedSceneHierarchy_AllContexts),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (All Contexts)", false, Priority_Item_Confirm_Inject_SelectedSceneHierarchy_AllContexts)]
        private static void Inject_SelectedSceneHierarchy_AllContexts(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_SelectedSceneHierarchy_AllContexts())
                return;

            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Selected Object Contexts Only)", false, Priority_Item_Inject_SelectedSceneHierarchies_SelectedObjectContextsOnly),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Selected Object Contexts Only)", false, Priority_Item_Inject_SelectedSceneHierarchies_SelectedObjectContextsOnly)]
        private static void Inject_SelectedSceneHierarchies_SelectedObjectContextsOnly(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd) || !DialogUtility.Injection.Confirm_Inject_SelectedSceneHierarchies_SelectedObjectContextsOnly())
                return;

            IEnumerable<GameObject> startObjects =
                Selection
                    .gameObjects
                    .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, ContextWalkFilter.SameAsStartObjects);
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Inject/Current Scene", true),
         MenuItem("Saneject/Inject/Current Scene", true)]
        private static bool Validate_Inject_CurrentScene()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab", true),
         MenuItem("Saneject/Inject/Current Prefab", true)]
        private static bool Validate_Inject_CurrentPrefab()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Objects", true),
         MenuItem("Saneject/Inject/All Scene Objects", true)]
        private static bool Validate_Inject_AllSceneObjects()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/All Scene Prefab Instances", true),
         MenuItem("Saneject/Inject/All Scene Prefab Instances", true)]
        private static bool Validate_Inject_AllScenePrefabInstances()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (All Contexts)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (All Contexts)", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_AllContexts()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Selected Object Contexts Only)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Selected Object Contexts Only)", true)]
        private static bool Validate_InjectSelectedSceneHierarchyStartObjectContextOnly()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        #endregion
    }
}