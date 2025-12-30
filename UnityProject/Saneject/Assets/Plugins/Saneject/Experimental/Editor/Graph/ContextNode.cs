using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class ContextNode : IEqualityComparer<ContextNode>
    {
        public ContextNode(Object obj)
        {
            GetContextTypeAndKey(obj, out (ContextType type, int key) result);
            ContextType = result.type;
            ContextKey = result.key;
            IsPrefab = result.type is ContextType.PrefabAsset or ContextType.PrefabInstance;
        }

        public ContextType ContextType { get; }
        public int ContextKey { get; }
        public bool IsPrefab { get; }

        public bool Equals(
            ContextNode x,
            ContextNode y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            if (x.GetType() != y.GetType())
                return false;

            return x.ContextKey == y.ContextKey;
        }

        public int GetHashCode(ContextNode obj)
        {
            return HashCode.Combine(obj.ContextKey);
        }

        private static void GetContextTypeAndKey(
            Object obj,
            out (ContextType type, int key) result)
        {
            GameObject gameObject = obj switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };

            // Non-GameObjects (ScriptableObjects, etc.)
            if (!gameObject)
            {
                result.type = ContextType.Global;
                result.key = 0;
                return;
            }

            // Prefab asset
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                result.type = ContextType.PrefabAsset;
                result.key = gameObject.transform.root.gameObject.GetInstanceID();
                return;
            }

            // Prefab instance
            GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            if (instanceRoot)
            {
                result.type = ContextType.PrefabInstance;
                result.key = instanceRoot.GetInstanceID();
                return;
            }

            result.type = ContextType.SceneObject;
            result.key = gameObject.scene.handle;
        }
    }
}