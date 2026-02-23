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
                        InjectionUtility.Inject
                        (
                            batchInjectorData,
                            sceneAssets: batchInjectorData
                                .SceneList
                                .GetEnabled()
                                .OfType<SceneAssetData>(),
                            prefabAssets: batchInjectorData
                                .PrefabList
                                .GetEnabled()
                                .OfType<PrefabAssetData>()
                        );
                }

                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        InjectionUtility.Inject
                        (
                            batchInjectorData,
                            sceneAssets: batchInjectorData
                                .SceneList
                                .GetEnabled()
                                .OfType<SceneAssetData>(),
                            prefabAssets: null
                        );
                }

                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        InjectionUtility.Inject
                        (
                            batchInjectorData,
                            sceneAssets: null,
                            prefabAssets: batchInjectorData
                                .PrefabList
                                .GetEnabled()
                                .OfType<PrefabAssetData>()
                        );
                }
            }
        }
    }
}