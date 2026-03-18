using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class MenuValidator
    {
        public static bool IsScene()
        {
            return SceneManager.sceneCount > 0 &&
                   !IsPrefabStage();
        }

        public static bool IsPrefabStage()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        public static bool HasSceneObjectSelection()
        {
            return GetSceneObjectSelectionCount() > 0;
        }

        public static int GetSceneObjectSelectionCount()
        {
            return Selection
                .gameObjects
                .Count(gameObject => gameObject.scene.IsValid());
        }

        public static bool HasValidBatchSelection()
        {
            return Selection
                .GetFiltered<Object>(SelectionMode.DeepAssets)
                .Any(selectedObject => selectedObject is GameObject or SceneAsset);
        }
    }
}
