using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class SceneHierarchyUtility
    {
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