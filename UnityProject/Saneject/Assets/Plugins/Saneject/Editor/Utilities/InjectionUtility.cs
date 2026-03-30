using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InjectionUtility
    {
        #region Batch injection

        public static void BatchInjectSelectedAssets(ContextWalkFilter walkFilter)
        {
            Object[] assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            List<BatchItem> batchItems = new();

            foreach (Object asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset);

                if (string.IsNullOrEmpty(path))
                    continue;

                BatchItem batchItem = AssetDatabase.LoadAssetAtPath<Object>(path) switch
                {
                    SceneAsset => new SceneBatchItem(path, walkFilter),
                    GameObject => new PrefabBatchItem(path, walkFilter),
                    _ => null
                };

                if (batchItem != null)
                    batchItems.Add(batchItem);
            }

            int sceneCount = batchItems.OfType<SceneBatchItem>().Count();
            int prefabCount = batchItems.OfType<PrefabBatchItem>().Count();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject_SelectedAssets(sceneCount, prefabCount))
                return;

            InjectionRunner.RunBatch
            (
                batchItems
            );
        }

        #endregion

        #region Scene injection

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

        public static void InjectSelectedSceneHierarchies(ContextWalkFilter walkFilter)
        {
            if (!DialogUtility.InjectionMenus.Confirm_InjectSelectedSceneHierarchies(walkFilter))
                return;

            InjectionRunner.Run
            (
                Selection.gameObjects.Where(go => go.scene.IsValid()),
                walkFilter
            );
        }

        #endregion

        #region Prefab asset injection

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
    }
}