using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InjectMenuItems
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + 100;

        private const int Priority_Group_CurrentScene = Priority_Base + 0;
        private const int Priority_Group_SelectedSceneHierarchies = Priority_Base + 5;
        private const int Priority_Group_CurrentPrefabAsset = Priority_Base + 10;

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

        [MenuItem("GameObject/Saneject/Inject Scene/All Contexts", false, Priority_Item_Inject_CurrentScene_All),
         MenuItem("Saneject/Inject Scene/All Contexts", false, Priority_Item_Inject_CurrentScene_All)]
        private static void Inject_CurrentScene_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Scene Objects", false, Priority_Item_Inject_CurrentScene_SceneObjects),
         MenuItem("Saneject/Inject Scene/Scene Objects", false, Priority_Item_Inject_CurrentScene_SceneObjects)]
        private static void Inject_CurrentScene_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Prefab Instances", false, Priority_Item_Inject_CurrentScene_PrefabInstances),
         MenuItem("Saneject/Inject Scene/Prefab Instances", false, Priority_Item_Inject_CurrentScene_PrefabInstances)]
        private static void Inject_CurrentScene_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/All Contexts", false, Priority_Item_Inject_SelectedSceneHierarchies_All),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/All Contexts", false, Priority_Item_Inject_SelectedSceneHierarchies_All)]
        private static void Inject_SelectedSceneHierarchy_AllContexts(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Scene Objects", false, Priority_Item_Inject_SelectedSceneHierarchies_SceneObjects),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Scene Objects", false, Priority_Item_Inject_SelectedSceneHierarchies_SceneObjects)]
        private static void Inject_SelectedSceneHierarchy_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Prefab Instances", false, Priority_Item_Inject_SelectedSceneHierarchies_PrefabInstances),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Prefab Instances", false, Priority_Item_Inject_SelectedSceneHierarchies_PrefabInstances)]
        private static void Inject_SelectedSceneHierarchy_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", false, Priority_Item_Inject_SelectedSceneHierarchies_SameAsSelection),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", false, Priority_Item_Inject_SelectedSceneHierarchies_SameAsSelection)]
        private static void Inject_SelectedSceneHierarchies_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Prefab asset menu items

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/All Contexts", false, Priority_Item_Inject_CurrentPrefabAsset_All),
         MenuItem("Saneject/Inject Prefab Asset/All Contexts", false, Priority_Item_Inject_CurrentPrefabAsset_All)]
        private static void Inject_CurrentPrefab_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Asset Objects", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabAssetObjects),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Asset Objects", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabAssetObjects)]
        private static void Inject_CurrentPrefab_PrefabAssetObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabAssetObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Instances", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabInstances),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Instances", false, Priority_Item_Inject_CurrentPrefabAsset_PrefabInstances)]
        private static void Inject_CurrentPrefab_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Same Contexts As Selection", false, Priority_Item_Inject_CurrentPrefabAsset_SameAsSelection),
         MenuItem("Saneject/Inject Prefab Asset/Same Contexts As Selection", false, Priority_Item_Inject_CurrentPrefabAsset_SameAsSelection)]
        private static void Inject_CurrentPrefab_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectPrefabAssetSelection(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Scene menu items validation

        [MenuItem("GameObject/Saneject/Inject Scene/All Contexts", true),
         MenuItem("Saneject/Inject Scene/All Contexts", true)]
        private static bool Validate_Inject_CurrentScene_All()
        {
            return MenuValidator.IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Scene Objects", true),
         MenuItem("Saneject/Inject Scene/Scene Objects", true)]
        private static bool Validate_Inject_CurrentScene_SceneObjects()
        {
            return MenuValidator.IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Prefab Instances", true),
         MenuItem("Saneject/Inject Scene/Prefab Instances", true)]
        private static bool Validate_Inject_CurrentScene_PrefabInstances()
        {
            return MenuValidator.IsScene();
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/All Contexts", true),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/All Contexts", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_AllContexts()
        {
            return MenuValidator.IsScene() &&
                   MenuValidator.HasSceneObjectSelection();
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Scene Objects", true),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Scene Objects", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_SceneObjects(MenuCommand cmd)
        {
            return MenuValidator.IsScene() &&
                   MenuValidator.HasSceneObjectSelection();
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Prefab Instances", true),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Prefab Instances", true)]
        private static bool Validate_Inject_SelectedSceneHierarchy_PrefabInstances(MenuCommand cmd)
        {
            return MenuValidator.IsScene() &&
                   MenuValidator.HasSceneObjectSelection();
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", true),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", true)]
        private static bool Validate_Inject_SelectedSceneHierarchies_SameAsSelection()
        {
            return MenuValidator.IsScene() &&
                   MenuValidator.HasSceneObjectSelection();
        }

        #endregion

        #region Prefab asset menu items validation

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/All Contexts", true),
         MenuItem("Saneject/Inject Prefab Asset/All Contexts", true)]
        private static bool Validate_Inject_CurrentPrefab_All(MenuCommand cmd)
        {
            return MenuValidator.IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Asset Objects", true),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Asset Objects", true)]
        private static bool Validate_Inject_CurrentPrefab_PrefabAssetObjects()
        {
            return MenuValidator.IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Instances", true),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Instances", true)]
        private static bool Validate_Inject_CurrentPrefab_PrefabInstances()
        {
            return MenuValidator.IsPrefabStage();
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Same Contexts As Selection", true),
         MenuItem("Saneject/Inject Prefab Asset/Same Contexts As Selection", true)]
        private static bool Validate_Inject_CurrentPrefab_SameAsSelection()
        {
            return MenuValidator.IsPrefabStage() &&
                   MenuValidator.HasSceneObjectSelection();
        }

        #endregion
    }
}
