using System.Linq;
using Plugins.Saneject.Experimental.Editor.Core;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectionMenuItems
    {
        #region Menu item methods

        [MenuItem("GameObject/Saneject/Inject Entire Scene", false, -10050),
         MenuItem("Saneject/Inject Entire Scene", false, -10050)]
        private static void InjectEntireScene()
        {
            GameObject[] startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            InjectionPipeline.Inject(startObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Entire Scene (Except Prefab Instances)", false, -10050),
         MenuItem("Saneject/Inject Entire Scene (Except Prefab Instances)", false, -10050)]
        private static void InjectEntireSceneExceptPrefabInstances()
        {
            GameObject[] startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects()
                .Where(obj => new ContextIdentity(obj).Type == ContextType.SceneObject)
                .ToArray();

            InjectionPipeline.Inject(startObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Selection (Full Walk)", false, -10000)]
        private static void InjectSelectionFullWalk()
        {
            InjectionPipeline.Inject(Selection.gameObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Selection (Context-Aware Walk)", false, -10000)]
        private static void InjectSelectionContextAwareWalk()
        {
            InjectionPipeline.Inject(Selection.gameObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("Assets/Saneject/Inject Selected Prefabs", false, -10000)]
        private static void InjectSelectedPrefabs()
        {
            InjectionPipeline.Inject(Selection.gameObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Open Prefab", false, -10000)]
        private static void InjectOpenPrefab()
        {
            GameObject[] startObjects =
            {
                PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot
            };

            InjectionPipeline.Inject(startObjects, WalkFilter.StartObjectsContext);
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Inject Entire Scene", true),
         MenuItem("Saneject/Inject Entire Scene", true)]
        private static bool InjectEntireScene_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Entire Scene (Except Prefab Instances)", true),
         MenuItem("Saneject/Inject Entire Scene (Except Prefab Instances)", true)]
        private static bool InjectEntireSceneExceptPrefabInstances_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selection (Full Walk)", true)]
        private static bool InjectSelectionFullWalk_Validate()
        {
            return Selection.gameObjects.Length > 0 && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selection (Context-Aware Walk)", true)]
        private static bool InjectSelectionContextAwareWalk_Validate()
        {
            return Selection.gameObjects.Length > 0 && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("Assets/Saneject/Inject Selected Prefabs", true)]
        private static bool InjectSelectedPrefabs_Validate()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem("GameObject/Saneject/Inject Open Prefab", true)]
        private static bool InjectOpenPrefab_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        #endregion
    }
}