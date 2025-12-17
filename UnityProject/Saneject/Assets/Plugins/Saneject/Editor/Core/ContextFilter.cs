using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    public static class ContextFilter
    {
        public static IEnumerable<Object> FilterInjectionCandidatesBySameContext(
            Object injectionTarget,
            IEnumerable<Object> candidates,
            out HashSet<Type> rejectedTypes)
        {
            rejectedTypes = new HashSet<Type>();

            if (!UserSettings.UseContextIsolation)
                return candidates;

            List<Object> filtered = new();

            foreach (Object obj in candidates)
                if (IsDependencyCandidateAllowed(injectionTarget, obj))
                    filtered.Add(obj);
                else
                    rejectedTypes.Add(obj.GetType());

            return filtered;
        }

        public static bool AreSameContext(
            Object a,
            Object b)
        {
            if (!UserSettings.UseContextIsolation)
                return true;

            GameObject aGameObject = GetGameObject(a);
            GameObject bGameObject = GetGameObject(b);

            // Non-GameObjects (ScriptableObjects, etc.) allowed in any context
            if (!aGameObject || !bGameObject)
                return true;

            if (aGameObject == bGameObject)
                return true;

            // Prefab asset check
            bool aIsAsset = PrefabUtility.IsPartOfPrefabAsset(aGameObject);
            bool bIsAsset = PrefabUtility.IsPartOfPrefabAsset(bGameObject);

            if (aIsAsset || bIsAsset)
            {
                if (!(aIsAsset && bIsAsset))
                    return false;

                GameObject aAssetRoot = aGameObject.transform.root.gameObject;
                GameObject bAssetRoot = bGameObject.transform.root.gameObject;

                return aAssetRoot == bAssetRoot;
            }

            // Scene check
            if (aGameObject.scene != bGameObject.scene)
                return false;

            // Prefab instance root check
            GameObject aInstance = PrefabUtility.GetNearestPrefabInstanceRoot(aGameObject);
            GameObject bInstance = PrefabUtility.GetNearestPrefabInstanceRoot(bGameObject);

            bool aIsInstance = aInstance;
            bool bIsInstance = bInstance;

            // One is prefab instance, other is plain scene object
            if (aIsInstance != bIsInstance)
                return false;

            // Both scene objects or both inside same instance root
            return !aIsInstance || aInstance == bInstance;
        }
 
        private static bool IsDependencyCandidateAllowed(
            Object target,
            Object candidate)
        {
            GameObject candidateGameObject = GetGameObject(candidate);
 
            bool candidateIsPrefabAsset = 
                candidate == candidateGameObject && 
                PrefabUtility.IsPartOfPrefabAsset(candidateGameObject) && 
                candidateGameObject.transform.root == candidateGameObject.transform;

            // Always allow prefabs themselves to be injected, like any other asset.
            return candidateIsPrefabAsset || AreSameContext(target, candidate);
        }

        private static GameObject GetGameObject(Object obj)
        {
            return obj switch
            {
                GameObject gameObject => gameObject,
                Component component => component.gameObject,
                _ => null
            };
        }
    }
}