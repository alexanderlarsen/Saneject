using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Core;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectMenuItems
    {
        #region Menu item methods

        [MenuItem("GameObject/Saneject/Inject Entire Scene", false, -10051),
         MenuItem("Saneject/Inject/Entire Scene", false, -10051)]
        private static void InjectEntireScene()
        {
            GameObject[] startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

            InjectionRunner.Run(startObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Entire Scene (Except Prefab Instances)", false, -10050),
         MenuItem("Saneject/Inject/Entire Scene (Except Prefab Instances)", false, -10050)]
        private static void InjectEntireSceneExceptPrefabInstances()
        {
            IEnumerable<GameObject> startObjects = SceneManager
                .GetActiveScene()
                .GetRootGameObjects()
                .Where(obj => new ContextIdentity(obj).Type == ContextType.SceneObject);

            InjectionRunner.Run(startObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Objects (Full Walk)", false, -10001),
         MenuItem("Saneject/Inject/Selected Scene Objects (Full Walk)", false, -10001)]
        private static void InjectSelectedSceneObjectsFullWalk()
        {
            IEnumerable<GameObject> startObjects = Selection
                .gameObjects
                .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, WalkFilter.All);
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Objects (Context-Aware Walk)", false, -10000),
         MenuItem("Saneject/Inject/Selected Scene Objects (Context-Aware Walk)", false, -10000)]
        private static void InjectSelectedSceneObjectsContextAwareWalk()
        {
            IEnumerable<GameObject> startObjects = Selection
                .gameObjects
                .Where(x => x.scene.IsValid());

            InjectionRunner.Run(startObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("GameObject/Saneject/Inject Currently Open Prefab", false, -9951),
         MenuItem("Saneject/Inject/Currently Open Prefab", false, -9951)]
        private static void InjectOpenPrefab()
        {
            GameObject[] startObjects = { PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot };

            InjectionRunner.Run(startObjects, WalkFilter.StartObjectsContext);
        }

        [MenuItem("Assets/Saneject/Inject Selected Prefab Assets", false, -9950),
         MenuItem("Saneject/Inject/Selected Prefab Assets", false, -9950)]
        private static void InjectSelectedPrefabs()
        {
            InjectionRunner.Run(Selection.gameObjects, WalkFilter.StartObjectsContext);
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Inject Entire Scene", true),
         MenuItem("Saneject/Inject/Entire Scene", true)]
        private static bool InjectEntireScene_Validate()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Entire Scene (Except Prefab Instances)", true),
         MenuItem("Saneject/Inject/Entire Scene (Except Prefab Instances)", true)]
        private static bool InjectEntireSceneExceptPrefabInstances_Validate()
        {
            return SceneManager.sceneCount > 0 &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Objects (Full Walk)", true),
         MenuItem("Saneject/Inject/Selected Scene Objects (Full Walk)", true)]
        private static bool InjectSelectionFullWalk_Validate()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Selected Scene Objects (Context-Aware Walk)", true),
         MenuItem("Saneject/Inject/Selected Scene Objects (Context-Aware Walk)", true)]
        private static bool InjectSelectionContextAwareWalk_Validate()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(go => go.scene.IsValid()) &&
                   PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        [MenuItem("GameObject/Saneject/Inject Currently Open Prefab", true),
         MenuItem("Saneject/Inject/Currently Open Prefab", true)]
        private static bool InjectOpenPrefab_Validate()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        [MenuItem("Assets/Saneject/Inject Selected Prefab Assets", true),
         MenuItem("Saneject/Inject/Selected Prefab Assets", true)]
        private static bool InjectSelectedPrefabs_Validate()
        {
            return Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.Any(PrefabUtility.IsPartOfPrefabAsset);
        }

        #endregion
    }
}