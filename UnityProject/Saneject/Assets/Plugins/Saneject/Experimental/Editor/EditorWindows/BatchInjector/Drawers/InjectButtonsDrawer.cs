using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class InjectButtonsDrawer
    {
        public static void DrawInjectButtons(BatchInjectorData data)
        {
            int sceneCount = data.sceneList.EnabledCount;
            int prefabCount = data.prefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        InjectAll(data);
                }

                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        InjectScenes(data);
                }

                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        InjectPrefabs(data);
                }
            }
        }

        #region Button action methods

        private static void InjectAll(BatchInjectorData data)
        {
            SceneBatchItem[] sceneBatchItems = data
                .sceneList
                .GetEnabled()
                .Select(asset => new SceneBatchItem(asset.GetAssetPath(), ContextWalkFilter.All))
                .ToArray();

            PrefabBatchItem[] prefabBatchItems = data
                .prefabList
                .GetEnabled()
                .Select(asset => new PrefabBatchItem(asset.GetAssetPath()))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, prefabBatchItems.Length))
                return;

            List<BatchItem> batchItems = new();
            batchItems.AddRange(sceneBatchItems);
            batchItems.AddRange(prefabBatchItems);
            InjectionRunner.RunBatch(batchItems.ToArray());
        }

        private static void InjectScenes(BatchInjectorData data)
        {
            SceneBatchItem[] sceneBatchItems = data
                .sceneList
                .GetEnabled()
                .Select(asset => new SceneBatchItem(asset.GetAssetPath(), ContextWalkFilter.All))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, 0))
                return;

            InjectionRunner.RunBatch(sceneBatchItems);
        }

        private static void InjectPrefabs(BatchInjectorData data)
        {
            PrefabBatchItem[] prefabBatchItems = data
                .prefabList
                .GetEnabled()
                .Select(asset => new PrefabBatchItem(asset.GetAssetPath()))
                .ToArray();

            if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(0, prefabBatchItems.Length))
                return;

            InjectionRunner.RunBatch(prefabBatchItems);
        }

        #endregion
    }
}