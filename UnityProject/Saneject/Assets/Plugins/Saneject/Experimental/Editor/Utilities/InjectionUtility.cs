using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class InjectionUtility
    {
        #region Scene

        public static void InjectCurrentScene(ContextWalkFilter walkFilter)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (!DialogUtility.InjectionMenus.Confirm_InjectCurrentScene(activeScene.name, walkFilter))
                return;

            InjectionRunner.Run
            (
                activeScene.GetRootGameObjects(),
                walkFilter
            );
        }

        public static void InjectSceneHierarchy(
            GameObject startObject,
            ContextWalkFilter walkFilter)
        {
            if (!DialogUtility.InjectionMenus.Confirm_InjectSceneHierarchy(walkFilter))
                return;

            InjectionRunner.Run
            (
                startObject.AsEnumerable(),
                walkFilter
            );
        }

        public static void InjectSelectedSceneHierarchies(ContextWalkFilter walkFilter)
        {
            if (!DialogUtility.InjectionMenus.Confirm_InjectSelectedSceneHierarchies(walkFilter))
                return;

            InjectionRunner.Run
            (
                Selection.gameObjects,
                walkFilter
            );
        }

        #endregion

        #region Prefab asset

        public static void InjectCurrentPrefabAsset(ContextWalkFilter walkFilter)
        {
            GameObject prefab = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

            if (!DialogUtility.InjectionMenus.Confirm_InjectCurrentPrefabAsset(prefab.name, walkFilter))
                return;

            InjectionRunner.Run
            (
                prefab.AsEnumerable(),
                walkFilter
            );
        }

        public static void InjectPrefabAssetHierarchy(
            GameObject startObject,
            ContextWalkFilter walkFilter)
        {
            GameObject prefab = startObject.transform.root.gameObject;

            if (!DialogUtility.InjectionMenus.Confirm_InjectCurrentPrefabAsset(prefab.name, walkFilter))
                return;

            InjectionRunner.Run
            (
                startObject.AsEnumerable(),
                walkFilter
            );
        }

        public static void InjectPrefabAssetSelection(ContextWalkFilter walkFilter)
        {
            GameObject prefab = Selection.gameObjects[0].transform.root.gameObject;

            if (!DialogUtility.InjectionMenus.Confirm_InjectCurrentPrefabAsset(prefab.name, walkFilter))
                return;

            InjectionRunner.Run
            (
                Selection.gameObjects,
                walkFilter
            );
        }

        #endregion

        public static void BatchInjectSelectedAssets()
        {
            BatchItem[] batchItems = Selection.GetFiltered<Object>(SelectionMode.DeepAssets)
                .CreateBatchItemsFromObjects(ContextWalkFilter.AllContexts)
                .ToArray();

            int sceneCount = batchItems.OfType<SceneBatchItem>().Count();
            int prefabCount = batchItems.OfType<PrefabBatchItem>().Count();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject_SelectedAssets(sceneCount, prefabCount))
                return;

            InjectionRunner.RunBatch
            (
                batchItems
            );
        }
    }
}