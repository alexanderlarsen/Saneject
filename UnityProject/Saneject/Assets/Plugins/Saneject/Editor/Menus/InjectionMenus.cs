using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Plugins.Saneject.Editor.Menus
{
    /// <summary>
    /// Adds editor menu items for injecting dependencies into scenes and prefabs.
    /// </summary>
    public class InjectionMenus
    {
        /// <summary>
        /// Validates if scene injection can be run (only outside of prefab editing).
        /// </summary>
        [MenuItem("Saneject/Inject Scene Dependencies", true, 49), MenuItem("GameObject/Inject Scene Dependencies", true, 49)]
        private static bool InjectSceneDependencies_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        /// <summary>
        /// Injects all dependencies into the active scene.
        /// </summary>
        [MenuItem("Saneject/Inject Scene Dependencies", false, 49), MenuItem("GameObject/Inject Scene Dependencies", false, 49)]
        private static void InjectSceneDependencies()
        {
            if (UserSettings.AskBeforeSceneInjection && !EditorUtility.DisplayDialog(
                    title: "Saneject: Inject Scene Dependencies",
                    message: "Are you sure you want to inject all dependencies in the scene?",
                    ok: "Yes",
                    cancel: "Cancel"))
                return;

            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            DependencyInjector.InjectCurrentScene();
        }

        /// <summary>
        /// Validates if prefab injection can be run (only while editing a prefab).
        /// </summary>
        [MenuItem("Saneject/Inject Prefab Dependencies", true, 50), MenuItem("GameObject/Inject Prefab Dependencies", true, 50)]
        private static bool InjectPrefabDependencies_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        /// <summary>
        /// Injects all dependencies into the open prefab.
        /// </summary>
        [MenuItem("Saneject/Inject Prefab Dependencies", false, 50), MenuItem("GameObject/Inject Prefab Dependencies", false, 50)]
        private static void InjectPrefabDependencies()
        {
            if (UserSettings.AskBeforePrefabInjection && !EditorUtility.DisplayDialog(
                    title: "Saneject: Inject Prefab Dependencies",
                    message: "Are you sure you want to inject all dependencies in the prefab?",
                    ok: "Yes",
                    cancel: "Cancel"))
                return;

            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            Scope scope = PrefabStageUtility.GetCurrentPrefabStage().FindComponentOfType<Scope>();
            DependencyInjector.InjectPrefab(scope);
        }
    }
}