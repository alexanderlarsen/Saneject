using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Editor.Core
{
    public static class LegacyContextFilter
    {
        /// <summary>
        /// Ensures that only dependencies from the same context as the injection target are kept.
        /// Scene objects only match within the same scene, prefab instances only within their root,
        /// and prefab assets only within their asset root. ScriptableObjects and other assets are unaffected.
        /// </summary>
        public static Object[] FilterBySameContext(
            Object[] objects,
            SerializedObject serializedObject,
            out HashSet<Type> rejectedTypes)
        {
            rejectedTypes = null;

            if (objects.Length == 0)
                return objects;

            object targetContext = GetContextKey(serializedObject.targetObject);
            List<Object> filtered = new();

            foreach (Object obj in objects)
            {
                if (obj is Component comp)
                {
                    object candidateContext = GetContextKey(comp);

                    // Reject if they don't share the same context
                    if (!Equals(targetContext, candidateContext))
                    {
                        rejectedTypes ??= new HashSet<Type>();
                        rejectedTypes.Add(comp.GetType());
                        continue;
                    }
                }

                // Non-Component objects (ScriptableObjects, etc.) are always valid
                filtered.Add(obj);
            }

            return filtered.ToArray();
        }

        /// <summary>
        /// Returns a context "key" that uniquely identifies whether an object belongs
        /// to a scene, a prefab instance, or a prefab asset. Prefab assets and their
        /// instances normalize to the same prefab asset root so they are treated as
        /// one context.
        /// </summary>
        private static object GetContextKey(Object obj)
        {
            if (obj is not Component component)
                return null; // unrestricted (ScriptableObjects, etc.)

            GameObject go = component.gameObject;

            // 1) Prefab INSTANCE
            if (PrefabUtility.IsPartOfPrefabInstance(go))
            {
                GameObject instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);

                if (instanceRoot)
                {
                    GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(instanceRoot);
                    return prefabAsset ? (Object)prefabAsset : instanceRoot;
                }

                return go.scene; // fallback
            }

            // 2) Prefab ASSET in Project window
            if (PrefabUtility.IsPartOfPrefabAsset(go))
                // The root of the prefab asset is just its transform.root
                return go.transform.root.gameObject;

            // 3) Open Prefab Stage
            PrefabStage stage = PrefabStageUtility.GetPrefabStage(go);

            if (stage && stage.prefabContentsRoot)
            {
                GameObject asset = PrefabUtility.GetCorrespondingObjectFromSource(stage.prefabContentsRoot);
                return asset ? (Object)asset : stage.prefabContentsRoot;
            }

            // 4) Scene object
            return go.scene;
        }
    }
}