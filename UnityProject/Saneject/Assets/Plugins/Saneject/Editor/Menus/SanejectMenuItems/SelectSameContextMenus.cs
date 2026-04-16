using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SelectSameContextMenus
    {
        #region Menu item methods

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Scene", false,
             SanejectMenuPriority.SelectSameContextObjects.InScene),
         MenuItem("Saneject/Select Same Context Objects/In Scene", false, SanejectMenuPriority.SelectSameContextObjects.InScene)]
        private static void SelectSameContextInScene(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            Object[] selection = SceneHierarchyUtility.GetSameContextInSceneSelection
            (
                Selection.gameObjects
            );

            Selection.objects = selection;
            SceneHierarchyUtility.Expand(selection);

            if (!ProjectSettings.UseContextIsolation)
                Debug.Log("Saneject: Context isolation is disabled in project settings. Selecting all GameObjects in the current scene.");
        }

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Hierarchy", false,
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy),
         MenuItem("Saneject/Select Same Context Objects/In Hierarchy", false,
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy)]
        private static void SelectSameContextInHierarchy(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            Object[] selection = SceneHierarchyUtility.GetSameContextInHierarchySelection
            (
                Selection.gameObjects
            );

            Selection.objects = selection;
            SceneHierarchyUtility.Expand(selection);

            if (!ProjectSettings.UseContextIsolation)
                Debug.Log("Saneject: Context isolation is disabled in project settings. Selecting all GameObjects in the selected hierarchy.");
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Scene", true,
             SanejectMenuPriority.SelectSameContextObjects.InScene),
         MenuItem("Saneject/Select Same Context Objects/In Scene", true, SanejectMenuPriority.SelectSameContextObjects.InScene)]
        private static bool Validate_SelectSameContextInScene()
        {
            return Selection
                .gameObjects
                .Select(x => x.transform.root)
                .Distinct()
                .Any(x => new ContextIdentity(x).Type is ContextType.SceneObject or ContextType.PrefabInstance);
        }

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Hierarchy", true,
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy),
         MenuItem("Saneject/Select Same Context Objects/In Hierarchy", true,
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy)]
        private static bool Validate_SelectSameContextInHierarchy()
        {
            return Selection
                .objects
                .Any(x => x is GameObject gameObject && gameObject.scene.IsValid());
        }

        #endregion
    }
}