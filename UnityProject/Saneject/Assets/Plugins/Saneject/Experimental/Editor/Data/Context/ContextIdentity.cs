using System;
using System.ComponentModel;
using System.Text;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Data.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ContextIdentity : IEquatable<ContextIdentity>
    {
        public ContextIdentity(Object obj)
        {
            ContextData data = GetContextData(obj);
            Type = data.Type;
            Id = data.Key;
            IsPrefab = data.Type is ContextType.PrefabAsset or ContextType.PrefabInstance;
        }

        public ContextType Type { get; }
        public int Id { get; }
        public bool IsPrefab { get; }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(ObjectNames.NicifyVariableName(Type.ToString()));
            sb.Append(" (");

            sb.Append(
                ProjectSettings.UseContextIsolation
                    ? $"ID: {Id}"
                    : "Context Isolation Off"
            );

            sb.Append(")");

            return sb.ToString();
        }

        private static ContextData GetContextData(Object obj)
        {
            GameObject gameObject = obj switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };

            if (!gameObject)
                return new ContextData(ContextType.Global, 0); // Non-GameObjects (ScriptableObjects, etc.)

            GameObject prefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            if (prefabInstanceRoot)
                return new ContextData(ContextType.PrefabInstance, prefabInstanceRoot.GetInstanceID()); // Prefab instance

            if (PrefabUtility.IsPartOfPrefabAsset(gameObject) || PrefabStageUtility.GetCurrentPrefabStage())
                return new ContextData(ContextType.PrefabAsset, gameObject.transform.root.gameObject.GetInstanceID()); // Prefab asset

            return new ContextData(ContextType.SceneObject, gameObject.scene.handle); // Scene object
        }

        private class ContextData
        {
            public ContextData(
                ContextType type,
                int key)
            {
                Type = type;
                Key = key;
            }

            public ContextType Type { get; }
            public int Key { get; }
        }

        #region Equality logic

        public bool Equals(ContextIdentity other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Type == other.Type &&
                   Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Id);
        }

        #endregion
    }
}