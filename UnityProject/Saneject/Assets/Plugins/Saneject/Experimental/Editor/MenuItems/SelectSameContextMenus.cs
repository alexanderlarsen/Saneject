using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class SelectSameContextMenus
    {
        #region Priority constants

        private const int Priority_Base = MenuPriority.Root + MenuPriority.Section * 3;

        private const int Priority_Group_Default = Priority_Base + MenuPriority.Group * 0;
        private const int Priority_Item_SelectSameContextInScene = Priority_Group_Default + 1;
        private const int Priority_Item_SelectSameContextInHierarchy = Priority_Group_Default + 2;

        #endregion

        #region Menu item methods

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Scene", false,
             Priority_Item_SelectSameContextInScene),
         MenuItem("Saneject/Select Same Context Objects/In Scene", false, Priority_Item_SelectSameContextInScene)]
        private static void SelectSameContextInScene()
        {
            IEnumerable<GameObject> validGameObjects = Object
                .FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => x.scene.IsValid());

            if (!UserSettings.UseContextIsolation)
            {
                Selection.objects = validGameObjects.ToArray<Object>();
                SceneHierarchyUtility.Expand(Selection.objects);
                Debug.Log("Saneject: Context isolation is disabled in user settings. Selecting all GameObjects in the current scene.");
                return;
            }

            HashSet<ContextIdentity> contextIdentities = Selection
                .gameObjects
                .Select(x => new ContextIdentity(x))
                .ToHashSet();

            Selection.objects = validGameObjects
                .Where(x => contextIdentities.Contains(new ContextIdentity(x)))
                .ToArray<Object>();

            SceneHierarchyUtility.Expand(Selection.objects);
        }

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Hierarchy", false,
             Priority_Item_SelectSameContextInHierarchy),
         MenuItem("Saneject/Select Same Context Objects/In Hierarchy", false,
             Priority_Item_SelectSameContextInHierarchy)]
        private static void SelectSameContextInHierarchy()
        {
            HashSet<GameObject> validGameObjects = Selection
                .gameObjects
                .Where(x => x.scene.IsValid())
                .ToHashSet();

            HashSet<Transform> roots = validGameObjects
                .Select(x => x.transform.root)
                .ToHashSet();

            if (!UserSettings.UseContextIsolation)
            {
                Selection.objects = roots
                    .SelectMany(x => x.GetComponentsInChildren<Transform>())
                    .Select(x => x.gameObject)
                    .ToArray<Object>();

                SceneHierarchyUtility.Expand(Selection.objects);
                Debug.Log("Saneject: Context isolation is disabled in user settings. Selecting all GameObjects in the selected hierarchy.");
                return;
            }

            HashSet<ContextIdentity> contextIdentities = validGameObjects
                .Select(x => new ContextIdentity(x))
                .ToHashSet();

            Selection.objects = roots
                .SelectMany(x => x.GetComponentsInChildren<Transform>())
                .Select(x => x.gameObject)
                .Where(x => contextIdentities.Contains(new ContextIdentity(x)))
                .ToArray<Object>();

            SceneHierarchyUtility.Expand(Selection.objects);
        }

        #endregion

        #region Validation methods

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Scene", true,
             Priority_Item_SelectSameContextInScene),
         MenuItem("Saneject/Select Same Context Objects/In Scene", true, Priority_Item_SelectSameContextInScene)]
        private static bool Validate_SelectSameContextInScene()
        {
            return Selection
                .objects
                .Any(x => x is GameObject gameObject && gameObject.scene.IsValid());
        }

        [MenuItem("GameObject/Saneject/Select Same Context Objects/In Hierarchy", true,
             Priority_Item_SelectSameContextInHierarchy),
         MenuItem("Saneject/Select Same Context Objects/In Hierarchy", true,
             Priority_Item_SelectSameContextInHierarchy)]
        private static bool Validate_SelectSameContextInHierarchy()
        {
            return Selection
                .objects
                .Any(x => x is GameObject gameObject && gameObject.scene.IsValid());
        }

        #endregion
    }
}