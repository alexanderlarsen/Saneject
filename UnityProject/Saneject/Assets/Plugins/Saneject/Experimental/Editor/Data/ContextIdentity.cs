using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class ContextIdentity : IEquatable<ContextIdentity>
    {
        public ContextIdentity(Object obj)
        {
            GetContextTypeAndKey(obj, out (ContextType type, int key) result);
            Type = result.type;
            Key = result.key;
            IsPrefab = result.type is ContextType.PrefabAsset or ContextType.PrefabInstance;
        }

        public ContextType Type { get; }
        public int Key { get; }
        public bool IsPrefab { get; }

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
            GameObject prefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            if (prefabInstanceRoot)
            {
                result.type = ContextType.PrefabInstance;
                result.key = prefabInstanceRoot.GetInstanceID();
                return;
            }

            result.type = ContextType.SceneObject;
            result.key = gameObject.scene.handle;
        }

        #region Equality logic

        public bool Equals(ContextIdentity other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Type == other.Type &&
                   Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Key);
        }

        #endregion
    }
}