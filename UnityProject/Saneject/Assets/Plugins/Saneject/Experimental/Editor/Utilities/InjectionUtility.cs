using System.Collections.Generic;
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
        public static class Inject
        {
            public static void CurrentScene()
            {
                Scene activeScene = SceneManager.GetActiveScene();

                if (!DialogUtility.InjectionMenus.Confirm_Inject_Scene(activeScene.name))
                    return;

                GameObject[] startObjects = activeScene
                    .GetRootGameObjects();

                InjectionRunner.Run(startObjects, ContextWalkFilter.AllContexts);
            }

            public static void CurrentPrefabAsset()
            {
                GameObject prefab = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

                if (!DialogUtility.InjectionMenus.Confirm_Inject_PrefabAsset(prefab.name))
                    return;

                GameObject[] startObjects =
                {
                    prefab
                };

                InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabAssets);
            }

            public static void AllSceneObjects()
            {
                Scene activeScene = SceneManager.GetActiveScene();

                if (!DialogUtility.InjectionMenus.Confirm_Inject_AllSceneObjects(activeScene.name))
                    return;

                IEnumerable<GameObject> startObjects = activeScene
                    .GetRootGameObjects();

                InjectionRunner.Run(startObjects, ContextWalkFilter.SceneObjects);
            }

            public static void AllScenePrefabInstances()
            {
                Scene activeScene = SceneManager.GetActiveScene();

                if (!DialogUtility.InjectionMenus.Confirm_Inject_AllScenePrefabInstances(activeScene.name))
                    return;

                IEnumerable<GameObject> startObjects = activeScene
                    .GetRootGameObjects();

                InjectionRunner.Run(startObjects, ContextWalkFilter.PrefabInstances);
            }

            public static void SelectedSceneHierarchies_AllContexts()
            {
                if (!DialogUtility.InjectionMenus.Confirm_Inject_SelectedSceneHierarchies_AllContexts())
                    return;

                IEnumerable<GameObject> startObjects =
                    Selection
                        .gameObjects
                        .Where(x => x.scene.IsValid());

                InjectionRunner.Run(startObjects, ContextWalkFilter.AllContexts);
            }

            public static void SelectedSceneHierarchies_SelectedObjectContextsOnly()
            {
                if (!DialogUtility.InjectionMenus.SelectedSceneHierarchies_SelectedObjectContextsOnly())
                    return;

                IEnumerable<GameObject> startObjects =
                    Selection
                        .gameObjects
                        .Where(x => x.scene.IsValid());

                InjectionRunner.Run(startObjects, ContextWalkFilter.SameAsSelection);
            }

            public static void Single_SceneHierarchy_ByContext(
                GameObject startObject,
                ContextWalkFilter walkFilter)
            {
                if (!DialogUtility.InjectionMenus.Confirm_Inject_Single_SceneHierarchy_ByContext(walkFilter))
                    return;

                GameObject[] startObjects =
                {
                    startObject
                };

                InjectionRunner.Run(startObjects, walkFilter);
            }
        }

        public static class BatchInject
        {
            public static void SelectedAssets()
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
}