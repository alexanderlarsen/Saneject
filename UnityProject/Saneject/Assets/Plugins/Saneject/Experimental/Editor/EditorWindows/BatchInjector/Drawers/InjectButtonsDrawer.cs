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
            int sceneCount = batchInjectorData.sceneList.EnabledCount;
            int prefabCount = batchInjectorData.prefabList.EnabledCount;

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(sceneCount == 0 && prefabCount == 0))
                {
                    if (GUILayout.Button("Inject All"))
                        InjectionUtility.Inject
                        (
                            sceneAssets: batchInjectorData.sceneList.GetEnabled(),
                            prefabAssets: batchInjectorData.prefabList.GetEnabled(),
                            onInjectionComplete: () => batchInjectorData.isDirty = true
                        );
                }

                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                        InjectionUtility.Inject
                        (
                            sceneAssets: batchInjectorData.sceneList.GetEnabled(),
                            prefabAssets: null,
                            onInjectionComplete: () => batchInjectorData.isDirty = true
                        );
                }

                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                        InjectionUtility.Inject
                        (
                            sceneAssets: null,
                            prefabAssets: batchInjectorData.prefabList.GetEnabled(),
                            onInjectionComplete: () => batchInjectorData.isDirty = true
                        );
                }
            }
        }
    }
}