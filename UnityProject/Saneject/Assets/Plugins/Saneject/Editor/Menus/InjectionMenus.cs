using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Runtime.Scopes;
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
            Scope scope = PrefabStageUtility.GetCurrentPrefabStage().FindComponentOfType<Scope>();
            DependencyInjector.InjectPrefab(scope);
        }
    }
}