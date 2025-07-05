using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Plugins.Saneject.Editor.Extensions
{
    /// <summary>
    /// Editor utility extensions for <see cref="GameObject" />s.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns true if the <see cref="GameObject" /> is part of a prefab asset or prefab editing stage.
        /// </summary>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <returns>True if this GameObject is a prefab or in a prefab editing context; otherwise false.</returns>
        public static bool IsPrefab(this GameObject gameObject)
        {
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

            return PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab ||
                   EditorUtility.IsPersistent(gameObject) ||
                   PrefabUtility.IsPartOfPrefabAsset(gameObject) ||
                   (stage && stage.IsPartOfPrefabContents(gameObject));
        }
    }
}