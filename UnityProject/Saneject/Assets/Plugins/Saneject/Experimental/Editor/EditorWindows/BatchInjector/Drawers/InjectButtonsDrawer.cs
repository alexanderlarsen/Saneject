using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    public static class InjectButtonsDrawer
    {
        public static void DrawInjectButtons(BatchInjectorData batchInjectorData)
        {
            int sceneCount = batchInjectorData.SceneList.EnabledCount;
            int prefabCount = batchInjectorData.PrefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        InjectAll(batchInjectorData);
                }

                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        InjectScenes(batchInjectorData);
                }

                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        InjectPrefabs(batchInjectorData);
                }
            }
        }

        private static void InjectAll(BatchInjectorData batchInjectorData)
        {
            IEnumerable<SceneAssetData> sceneAssetData = batchInjectorData.SceneList.GetEnabled().OfType<SceneAssetData>();
            IEnumerable<PrefabAssetData> prefabAssetData = batchInjectorData.PrefabList.GetEnabled().OfType<PrefabAssetData>();

            if (InjectionUtility.TryInject(sceneAssetData, prefabAssetData))
            {
                batchInjectorData.SceneList.Sort();
                batchInjectorData.PrefabList.Sort();
                batchInjectorData.IsDirty = true;
            }
        }

        private static void InjectScenes(BatchInjectorData batchInjectorData)
        {
            IEnumerable<SceneAssetData> sceneAssetData = batchInjectorData.SceneList.GetEnabled().OfType<SceneAssetData>();

            if (InjectionUtility.TryInject(sceneAssetData, null))
            {
                batchInjectorData.SceneList.Sort();
                batchInjectorData.IsDirty = true;
            }
        }

        private static void InjectPrefabs(BatchInjectorData batchInjectorData)
        {
            IEnumerable<PrefabAssetData> prefabAssetData = batchInjectorData.PrefabList.GetEnabled().OfType<PrefabAssetData>();

            if (InjectionUtility.TryInject(null, prefabAssetData))
            {
                batchInjectorData.PrefabList.Sort();
                batchInjectorData.IsDirty = true;
            }
        }
    }
}