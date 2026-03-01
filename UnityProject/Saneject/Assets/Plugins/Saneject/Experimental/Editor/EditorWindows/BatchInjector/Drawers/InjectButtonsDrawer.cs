using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Controls;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Drawers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class InjectButtonsDrawer
    {
        public static void DrawInjectButtons(
            BatchInjectorData batchInjectorData,
            ReorderableAssetList reorderableSceneList,
            ReorderableAssetList reorderablePrefabList)
        {
            int sceneCount = batchInjectorData.SceneList.EnabledCount;
            int prefabCount = batchInjectorData.PrefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        InjectAll(batchInjectorData, reorderableSceneList, reorderablePrefabList);
                }

                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        InjectScenes(batchInjectorData, reorderableSceneList);
                }

                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        InjectPrefabs(batchInjectorData, reorderablePrefabList);
                }
            }
        }

        private static void InjectAll(
            BatchInjectorData batchInjectorData,
            ReorderableAssetList reorderableSceneList,
            ReorderableAssetList reorderablePrefabList)
        {
            IEnumerable<SceneAssetData> sceneAssetData = batchInjectorData.SceneList.GetEnabled().OfType<SceneAssetData>();
            IEnumerable<PrefabAssetData> prefabAssetData = batchInjectorData.PrefabList.GetEnabled().OfType<PrefabAssetData>();

            if (!InjectionUtility.TryInject(sceneAssetData, prefabAssetData))
                return;

            batchInjectorData.SceneList.Sort();
            batchInjectorData.PrefabList.Sort();
            batchInjectorData.IsDirty = true;
            reorderableSceneList.ClearSelection();
            reorderablePrefabList.ClearSelection();
        }

        private static void InjectScenes(
            BatchInjectorData batchInjectorData,
            ReorderableAssetList reorderableSceneList)
        {
            IEnumerable<SceneAssetData> sceneAssetData = batchInjectorData.SceneList.GetEnabled().OfType<SceneAssetData>();

            if (!InjectionUtility.TryInject(sceneAssetData, null))
                return;

            batchInjectorData.SceneList.Sort();
            batchInjectorData.IsDirty = true;
            reorderableSceneList.ClearSelection();
        }

        private static void InjectPrefabs(
            BatchInjectorData batchInjectorData,
            ReorderableAssetList reorderablePrefabList)
        {
            IEnumerable<PrefabAssetData> prefabAssetData = batchInjectorData.PrefabList.GetEnabled().OfType<PrefabAssetData>();

            if (!InjectionUtility.TryInject(null, prefabAssetData))
                return;

            batchInjectorData.PrefabList.Sort();
            batchInjectorData.IsDirty = true;
            reorderablePrefabList.ClearSelection();
        }
    }
}