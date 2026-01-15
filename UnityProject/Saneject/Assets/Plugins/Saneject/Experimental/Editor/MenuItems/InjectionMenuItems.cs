using System.Linq;
using Plugins.Saneject.Experimental.Editor.Core;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectionMenuItems
    {
        [MenuItem("GameObject/Saneject/Inject Entire Scene", false, -10001),
         MenuItem("Saneject/Inject Entire Scene", false, -10001)]
        private static void InjectEntireScene()
        {
            GameObject[] startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            InjectionPipeline.Inject(startObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Entire Scene (Excluding Prefab Instances)", false, -10001),
         MenuItem("Saneject/Inject Entire Scene (Excluding Prefab Instances)", false, -10001)]
        private static void InjectEntireSceneExcludingPrefabInstances()
        {
            GameObject[] startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects()
                .Where(o => new ContextIdentity(o).Type == ContextType.SceneObject)
                .ToArray();

            InjectionPipeline.Inject(startObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Full Hierarchy", false, -10001)]
        private static void InjectFullHierarchy()
        {
            InjectionPipeline.Inject(Selection.gameObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Selection", false, -10000), MenuItem("Assets/Saneject/Inject Selection", false, -10000)]
        private static void InjectSelection()
        {
            InjectionPipeline.Inject(Selection.gameObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Full Hierarchy", true, -10001),
         MenuItem("GameObject/Saneject/Inject Selection", true, -10000),
         MenuItem("Assets/Saneject/Inject Selection", true, -10000)]
        private static bool ValidateHasSelection()
        {
            return Selection.gameObjects.Length > 0;
        }
    }
}