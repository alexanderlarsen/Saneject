using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SceneHierarchyUtility
    {
        public static Object[] GetSameContextInSceneSelection(IEnumerable<GameObject> selectedGameObjects)
        {
            HashSet<GameObject> selected = selectedGameObjects?
                .Where(x => x)
                .ToHashSet() ?? new HashSet<GameObject>();

            IEnumerable<GameObject> validGameObjects = Object
                .FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => new ContextIdentity(x).Type is ContextType.SceneObject or ContextType.PrefabInstance);

            if (!ProjectSettings.UseContextIsolation)
                return validGameObjects.ToArray<Object>();

            HashSet<ContextIdentity> contextIdentities = selected
                .Select(x => new ContextIdentity(x))
                .ToHashSet();

            return validGameObjects
                .Where(x => contextIdentities.Contains(new ContextIdentity(x)))
                .ToArray<Object>();
        }

        public static Object[] GetSameContextInHierarchySelection(IEnumerable<GameObject> selectedGameObjects)
        {
            HashSet<GameObject> selected = selectedGameObjects?
                .Where(x => x)
                .ToHashSet() ?? new HashSet<GameObject>();

            HashSet<Transform> roots = selected
                .Select(x => x.transform.root)
                .ToHashSet();

            if (!ProjectSettings.UseContextIsolation)
                return roots
                    .SelectMany(x => x.GetComponentsInChildren<Transform>())
                    .Select(x => x.gameObject)
                    .ToArray<Object>();

            HashSet<ContextIdentity> contextIdentities = selected
                .Select(x => new ContextIdentity(x))
                .ToHashSet();

            return roots
                .SelectMany(x => x.GetComponentsInChildren<Transform>())
                .Select(x => x.gameObject)
                .Where(x => contextIdentities.Contains(new ContextIdentity(x)))
                .ToArray<Object>();
        }

        public static void Expand(Object[] objects)
        {
            try
            {
                if (objects == null || objects.Length == 0)
                    return;

                Type hierarchyType = typeof(EditorWindow)
                    .Assembly
                    .GetType("UnityEditor.SceneHierarchyWindow");

                EditorWindow window = EditorWindow.GetWindow(hierarchyType);

                MethodInfo setExpanded = hierarchyType.GetMethod
                (
                    "SetExpanded",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                foreach (Object obj in objects)
                {
                    if (obj is not GameObject gameObject)
                        continue;

                    // Expand ancestors first (but not siblings / children)
                    Transform current = gameObject.transform.parent;

                    while (current)
                    {
                        setExpanded!.Invoke(window, new object[]
                        {
                            current.gameObject.GetInstanceID(),
                            true
                        });

                        current = current.parent;
                    }

                    // Finally expand the object itself
                    setExpanded!.Invoke(window, new object[]
                    {
                        gameObject.GetInstanceID(),
                        true
                    });
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Saneject: Could not expand the scene hierarchy for same context selection. This feature relies on internal Unity editor APIs which may be unavailable in this Unity version.");
            }
        }
    }
}