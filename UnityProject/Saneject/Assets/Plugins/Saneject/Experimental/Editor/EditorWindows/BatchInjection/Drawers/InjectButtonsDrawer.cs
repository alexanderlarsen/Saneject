using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Data;
using Plugins.Saneject.Experimental.Editor.Pipeline;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Drawers
{
    public static class InjectButtonsDrawer
    {
        public static void DrawInjectButtons(BatchInjectorData injectorData)
        {
            int sceneCount = injectorData.sceneList.EnabledCount;
            int prefabCount = injectorData.prefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                // Inject All
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                    {
                        SceneBatchItem[] sceneBatchItems = injectorData
                            .sceneList
                            .GetEnabled()
                            .Select(asset => new SceneBatchItem(asset.Path, ContextWalkFilter.All))
                            .ToArray();

                        PrefabBatchItem[] prefabBatchItems = injectorData
                            .prefabList
                            .GetEnabled()
                            .Select(asset => new PrefabBatchItem(asset.Path))
                            .ToArray();

                        if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, prefabBatchItems.Length))
                            return;

                        List<BatchItem> batchItems = new();
                        batchItems.AddRange(sceneBatchItems);
                        batchItems.AddRange(prefabBatchItems);
                        InjectionRunner.RunBatch(batchItems.ToArray());
                    }
                }

                // Inject Scenes
                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                    {
                        SceneBatchItem[] sceneBatchItems = injectorData
                            .sceneList
                            .GetEnabled()
                            .Select(asset => new SceneBatchItem(asset.Path, ContextWalkFilter.All))
                            .ToArray();

                        if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(sceneBatchItems.Length, 0))
                            return;
 
                        InjectionRunner.RunBatch(sceneBatchItems.ToArray());
                    }
                }

                // Inject Prefabs
                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                    {
                        PrefabBatchItem[] prefabBatchItems = injectorData
                            .prefabList
                            .GetEnabled()
                            .Select(asset => new PrefabBatchItem(asset.Path))
                            .ToArray();

                        if (!DialogUtility.BatchInjectionMenus.Confirm_BatchInject(0, prefabBatchItems.Length))
                            return;
 
                        InjectionRunner.RunBatch(prefabBatchItems.ToArray());
                    }
                }
            }
        }
    }
}