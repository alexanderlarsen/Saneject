using Plugins.Saneject.Editor.BatchInjection.Core;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.Drawers
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
                        if (!Dialogs.BatchInjection.ConfirmInjectAll(sceneCount, prefabCount))
                            return;

                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            return;

                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();

                        BatchInjector.BatchInjectAllScenesAndPrefabs
                        (
                            sceneAssets: injectorData.sceneList.GetEnabled(),
                            prefabAssets: injectorData.prefabList.GetEnabled()
                        );
                    }
                }

                // Inject Scenes
                using (new EditorGUI.DisabledScope(sceneCount == 0))
                {
                    if (GUILayout.Button($"Inject Scenes ({sceneCount})"))
                    {
                        if (!Dialogs.BatchInjection.ConfirmInjectScene(sceneCount))
                            return;

                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            return;

                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();

                        BatchInjector.BatchInjectScenes
                        (
                            sceneAssets: injectorData.sceneList.GetEnabled(),
                            logStats: true
                        );
                    }
                }

                // Inject Prefabs
                using (new EditorGUI.DisabledScope(prefabCount == 0))
                {
                    if (GUILayout.Button($"Inject Prefabs ({prefabCount})"))
                    {
                        if (!Dialogs.BatchInjection.ConfirmInjectPrefab(prefabCount))
                            return;

                        if (UserSettings.ClearLogsOnInjection)
                            ConsoleUtils.ClearLog();

                        BatchInjector.BatchInjectPrefabs
                        (
                            prefabAssets: injectorData.prefabList.GetEnabled(),
                            logStats: true
                        );
                    }
                }
            }
        }
    }
}