using System.Collections.Generic;
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

            IEnumerable<GameObject> validGameObjects = Object
                .FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => new ContextIdentity(x).Type is ContextType.SceneObject or ContextType.PrefabInstance);

            if (!ProjectSettings.UseContextIsolation)
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
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy),
         MenuItem("Saneject/Select Same Context Objects/In Hierarchy", false,
             SanejectMenuPriority.SelectSameContextObjects.InHierarchy)]
        private static void SelectSameContextInHierarchy(MenuCommand cmd)
        {
            if (!MenuCommandUtility.IsFirstInvocation(cmd))
                return;

            HashSet<GameObject> validGameObjects = Selection
                .gameObjects
                .ToHashSet();

            HashSet<Transform> roots = validGameObjects
                .Select(x => x.transform.root)
                .ToHashSet();

            if (!ProjectSettings.UseContextIsolation)
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
