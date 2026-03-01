using System.ComponentModel;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 1;

        private const int Priority_Group_CurrentScene = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Group_SelectedSceneHierarchies = Priority_Base + MenuPriority.Group * 1;

        private const int Priority_Group_CurrentPrefabAsset = Priority_Base + MenuPriority.Group * 2;

        private const int Priority_Item_Inject_CurrentScene_All = Priority_Group_CurrentScene + 1;
        private const int Priority_Item_Inject_CurrentScene_SceneObjects = Priority_Group_CurrentScene + 2;
        private const int Priority_Item_Inject_CurrentScene_PrefabInstances = Priority_Group_CurrentScene + 3;

        private const int Priority_Item_Inject_SelectedSceneHierarchies_All = Priority_Group_SelectedSceneHierarchies + 1;
        private const int Priority_Item_Inject_SelectedSceneHierarchies_SceneObjects = Priority_Group_SelectedSceneHierarchies + 2;
        private const int Priority_Item_Inject_SelectedSceneHierarchies_PrefabInstances = Priority_Group_SelectedSceneHierarchies + 3;
        private const int Priority_Item_Inject_SelectedSceneHierarchies_SameAsSelection = Priority_Group_SelectedSceneHierarchies + 4;

        private const int Priority_Item_Inject_CurrentPrefabAsset_All = Priority_Group_CurrentPrefabAsset + 1;
        private const int Priority_Item_Inject_CurrentPrefabAsset_PrefabAssetObjects = Priority_Group_CurrentPrefabAsset + 2;
        private const int Priority_Item_Inject_CurrentPrefabAsset_PrefabInstances = Priority_Group_CurrentPrefabAsset + 3;
        private const int Priority_Item_Inject_CurrentPrefabAsset_SameAsSelection = Priority_Group_CurrentPrefabAsset + 4;

        #endregion

        #region Scene menu items

        [MenuItem("GameObject/Saneject/Inject/Current Scene (All Contexts)", false, Priority_Item_Inject_CurrentScene_All),
         MenuItem("Saneject/Inject/Current Scene (All Contexts)", false, Priority_Item_Inject_CurrentScene_All)]
        private static void Inject_CurrentScene_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Scene (Scene Objects)", false, Priority_Item_Inject_CurrentScene_SceneObjects),
         MenuItem("Saneject/Inject/Current Scene (Scene Objects)", false, Priority_Item_Inject_CurrentScene_SceneObjects)]
        private static void Inject_CurrentScene_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Scene (Prefab Instances)", false, Priority_Item_Inject_CurrentScene_PrefabInstances),
         MenuItem("Saneject/Inject/Current Scene (Prefab Instances)", false, Priority_Item_Inject_CurrentScene_PrefabInstances)]
        private static void Inject_CurrentScene_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (All Contexts)", false, Priority_Item_Inject_SelectedSceneHierarchies_All),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (All Contexts)", false, Priority_Item_Inject_SelectedSceneHierarchies_All)]
        private static void Inject_SelectedSceneHierarchy_AllContexts(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Scene Objects)", false, Priority_Item_Inject_SelectedSceneHierarchies_SceneObjects),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Scene Objects)", false, Priority_Item_Inject_SelectedSceneHierarchies_SceneObjects)]
        private static void Inject_SelectedSceneHierarchy_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Prefab Instances)", false, Priority_Item_Inject_SelectedSceneHierarchies_PrefabInstances),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Prefab Instances)", false, Priority_Item_Inject_SelectedSceneHierarchies_PrefabInstances)]
        private static void Inject_SelectedSceneHierarchy_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Same Contexts As Selection)", false, Priority_Item_Inject_SelectedSceneHierarchies_SameAsSelection),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Same Contexts As Selection)", false, Priority_Item_Inject_SelectedSceneHierarchies_SameAsSelection)]
        private static void Inject_SelectedSceneHierarchies_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Prefab asset menu items

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (All Contexts)", false, Priority_Item_Inject_CurrentPrefabAsset_All),
         MenuItem("Saneject/Inject/Current Prefab Asset (All Contexts)", false, Priority_Item_Inject_CurrentPrefabAsset_All)]
        private static void Inject_CurrentPrefab_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Prefab Asset Objects)", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabAssetObjects),
         MenuItem("Saneject/Inject/Current Prefab Asset (Prefab Asset Objects)", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabAssetObjects)]
        private static void Inject_CurrentPrefab_PrefabAssetObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabAssetObjects);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Prefab Instances)", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabInstances),
         MenuItem("Saneject/Inject/Current Prefab Asset (Prefab Instances)", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabInstances)]
        private static void Inject_CurrentPrefab_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Same Contexts As Selection)", false, Priority_Item_Inject_CurrentPrefabAsset_SameAsSelection),
         MenuItem("Saneject/Inject/Current Prefab Asset (Same Contexts As Selection)", false, Priority_Item_Inject_CurrentPrefabAsset_SameAsSelection)]
        private static void Inject_CurrentPrefab_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectPrefabAssetSelection(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Scene menu items validation

        [MenuItem("GameObject/Saneject/Inject/Current Scene (All Contexts)", true),
         MenuItem("Saneject/Inject/Current Scene (All Contexts)", true)]
        private static bool Validate_Inject_CurrentScene_All()
        {
            return IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject/Current Scene (Scene Objects)", true),
         MenuItem("Saneject/Inject/Current Scene (Scene Objects)", true)]
        private static bool Validate_Inject_CurrentScene_SceneObjects()
        {
            return IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject/Current Scene (Prefab Instances)", true),
         MenuItem("Saneject/Inject/Current Scene (Prefab Instances)", true)]
        private static bool Validate_Inject_CurrentScene_PrefabInstances()
        {
            return IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (All Contexts)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (All Contexts)", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_AllContexts()
        {
            return IsScene() && HasSelectedGameObjects();
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Scene Objects)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Scene Objects)", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_SceneObjects(MenuCommand cmd)
        {
            return IsScene() && HasSelectedGameObjects();
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Prefab Instances)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Prefab Instances)", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_PrefabInstances(MenuCommand cmd)
        {
            return IsScene() && HasSelectedGameObjects();
        }

        [MenuItem("GameObject/Saneject/Inject/Selected Scene Hierarchies (Same Contexts As Selection)", true),
         MenuItem("Saneject/Inject/Selected Scene Hierarchies (Same Contexts As Selection)", true)]
        private static bool Validate_Inject_SelectedSceneHierarchies_SameAsSelection()
        {
            return IsScene() && HasSelectedGameObjects();
        }

        #endregion

        #region Prefab asset menu items validation

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (All Contexts)", true),
         MenuItem("Saneject/Inject/Current Prefab Asset (All Contexts)", true)]
        private static bool Validate_Inject_CurrentPrefab_All(MenuCommand cmd)
        {
            return IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Prefab Asset Objects)", true),
         MenuItem("Saneject/Inject/Current Prefab Asset (Prefab Asset Objects)", true)]
        private static bool Validate_Inject_CurrentPrefab_PrefabAssetObjects()
        {
            return IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Prefab Instances)", true),
         MenuItem("Saneject/Inject/Current Prefab Asset (Prefab Instances)", true)]
        private static bool Validate_Inject_CurrentPrefab_PrefabInstances()
        {
            return IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject/Current Prefab Asset (Same Contexts As Selection)", true),
         MenuItem("Saneject/Inject/Current Prefab Asset (Same Contexts As Selection)", true)]
        private static bool Validate_Inject_CurrentPrefab_SameAsSelection()
        {
            return IsPrefabStage() && HasSelectedGameObjects();
        }

        #endregion

        #region Validation helpers

        private static bool IsScene()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        private static bool IsPrefabStage()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        private static bool HasSelectedGameObjects()
        {
            return Selection.gameObjects.Length > 0;
        }

        #endregion
    }
}