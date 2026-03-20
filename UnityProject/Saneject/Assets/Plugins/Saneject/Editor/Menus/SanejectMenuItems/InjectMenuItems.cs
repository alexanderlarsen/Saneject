using System.ComponentModel;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InjectMenuItems
    {
        #region Scene menu items

        [MenuItem("GameObject/Saneject/Inject Scene/All Contexts", false, SanejectMenuPriority.InjectScene.AllContexts),
         MenuItem("Saneject/Inject Scene/All Contexts", false, SanejectMenuPriority.InjectScene.AllContexts)]
        private static void Inject_CurrentScene_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Scene Objects", false, SanejectMenuPriority.InjectScene.SceneObjects),
         MenuItem("Saneject/Inject Scene/Scene Objects", false, SanejectMenuPriority.InjectScene.SceneObjects)]
        private static void Inject_CurrentScene_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Scene/Prefab Instances", false, SanejectMenuPriority.InjectScene.PrefabInstances),
         MenuItem("Saneject/Inject Scene/Prefab Instances", false, SanejectMenuPriority.InjectScene.PrefabInstances)]
        private static void Inject_CurrentScene_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentScene(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/All Contexts", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.AllContexts),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/All Contexts", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.AllContexts)]
        private static void Inject_SelectedSceneHierarchy_AllContexts(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Scene Objects", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.SceneObjects),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Scene Objects", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.SceneObjects)]
        private static void Inject_SelectedSceneHierarchy_SceneObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Prefab Instances", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.PrefabInstances),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Prefab Instances", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.PrefabInstances)]
        private static void Inject_SelectedSceneHierarchy_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.SameContextsAsSelection),
         MenuItem("Saneject/Inject Selected Scene Hierarchies/Same Contexts As Selection", false, SanejectMenuPriority.InjectSelectedSceneHierarchies.SameContextsAsSelection)]
        private static void Inject_SelectedSceneHierarchies_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectSelectedSceneHierarchies(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Prefab asset menu items

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/All Contexts", false, SanejectMenuPriority.InjectPrefabAsset.AllContexts),
         MenuItem("Saneject/Inject Prefab Asset/All Contexts", false, SanejectMenuPriority.InjectPrefabAsset.AllContexts)]
        private static void Inject_CurrentPrefab_All(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.AllContexts);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Asset Objects", false, SanejectMenuPriority.InjectPrefabAsset.PrefabAssetObjects),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Asset Objects", false, SanejectMenuPriority.InjectPrefabAsset.PrefabAssetObjects)]
        private static void Inject_CurrentPrefab_PrefabAssetObjects(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabAssetObjects);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Prefab Instances", false, SanejectMenuPriority.InjectPrefabAsset.PrefabInstances),
         MenuItem("Saneject/Inject Prefab Asset/Prefab Instances", false, SanejectMenuPriority.InjectPrefabAsset.PrefabInstances)]
        private static void Inject_CurrentPrefab_PrefabInstances(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectCurrentPrefabAsset(ContextWalkFilter.PrefabInstances);
        }

        [MenuItem("GameObject/Saneject/Inject Prefab Asset/Same Contexts As Selection", false, SanejectMenuPriority.InjectPrefabAsset.SameContextsAsSelection),
         MenuItem("Saneject/Inject Prefab Asset/Same Contexts As Selection", false, SanejectMenuPriority.InjectPrefabAsset.SameContextsAsSelection)]
        private static void Inject_CurrentPrefab_SameAsSelection(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            InjectionUtility.InjectPrefabAssetSelection(ContextWalkFilter.SameContextsAsSelection);
        }

        #endregion

        #region Batch inject selected assets methods

        [MenuItem("Assets/Saneject/Batch Inject Selected Assets/All Contexts", false, SanejectMenuPriority.BatchInjectSelectedAssets.AllContexts),
         MenuItem("Saneject/Batch Inject Selected Assets/All Contexts", false, SanejectMenuPriority.BatchInjectSelectedAssets.AllContexts)]
        private static void BatchInject_SelectedAssets_AllContexts(MenuCommand cmd)
        {
            InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.AllContexts);
        }

        [MenuItem("Assets/Saneject/Batch Inject Selected Assets/Scene Objects", false, SanejectMenuPriority.BatchInjectSelectedAssets.SceneObjects),
         MenuItem("Saneject/Batch Inject Selected Assets/Scene Objects", false, SanejectMenuPriority.BatchInjectSelectedAssets.SceneObjects)]
        private static void BatchInject_SelectedAssets_SceneObjects(MenuCommand cmd)
        {
            InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.SceneObjects);
        }

        [MenuItem("Assets/Saneject/Batch Inject Selected Assets/Prefab Asset Objects", false, SanejectMenuPriority.BatchInjectSelectedAssets.PrefabAssetObjects),
         MenuItem("Saneject/Batch Inject Selected Assets/Prefab Asset Objects", false, SanejectMenuPriority.BatchInjectSelectedAssets.PrefabAssetObjects)]
        private static void BatchInject_SelectedAssets_PrefabAssetObjects(MenuCommand cmd)
        {
            InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.PrefabAssetObjects);
        }

        [MenuItem("Assets/Saneject/Batch Inject Selected Assets/Prefab Instances", false, SanejectMenuPriority.BatchInjectSelectedAssets.PrefabInstances),
         MenuItem("Saneject/Batch Inject Selected Assets/Prefab Instances", false, SanejectMenuPriority.BatchInjectSelectedAssets.PrefabInstances)]
        private static void BatchInject_SelectedAssets_PrefabInstances(MenuCommand cmd)
        {
            InjectionUtility.BatchInjectSelectedAssets(ContextWalkFilter.PrefabInstances);
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

        #region Batch inject selected assets validation methods

        [MenuItem("Assets/Saneject/Batch Inject Selected Assets/All Contexts", true),
         MenuItem("Saneject/Batch Inject Selected Assets/All Contexts", true),
         MenuItem("Assets/Saneject/Batch Inject Selected Assets/Scene Objects", true),
         MenuItem("Saneject/Batch Inject Selected Assets/Scene Objects", true),
         MenuItem("Assets/Saneject/Batch Inject Selected Assets/Prefab Asset Objects", true),
         MenuItem("Saneject/Batch Inject Selected Assets/Prefab Asset Objects", true),
         MenuItem("Assets/Saneject/Batch Inject Selected Assets/Prefab Instances", true),
         MenuItem("Saneject/Batch Inject Selected Assets/Prefab Instances", true)]
        private static bool Validate_BatchInject_SelectedAssets()
        {
            return MenuValidator.HasValidBatchSelection();
        }

        #endregion
    }
}