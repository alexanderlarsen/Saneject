using System;
using System.ComponentModel;
using System.Text;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Data.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ContextIdentity : IEquatable<ContextIdentity>
    {
        public ContextIdentity(Object obj)
        {
            ContextData data = GetContextData(obj);
            Type = data.Type;
            Id = data.Key;
            ContainerType = data.ContainerType;
            ContainerId = data.ContainerKey;
            IsPrefab = data.Type is ContextType.PrefabAsset or ContextType.PrefabInstance;
        }

        public ContextType Type { get; }
        public int Id { get; }
        public ContextType ContainerType { get; }
        public int ContainerId { get; }
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
                throw new ArgumentException(
                    $"ContextIdentity requires a GameObject or Component, got {obj?.GetType().Name ?? "null"}",
                    nameof(obj));

            PrefabStage currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            bool isInCurrentPrefabStage = currentPrefabStage && gameObject.scene == currentPrefabStage.scene;
            bool isPrefabAssetObject = PrefabUtility.IsPartOfPrefabAsset(gameObject) || isInCurrentPrefabStage;
            GameObject prefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

            GameObject prefabAssetRoot = isInCurrentPrefabStage
                ? currentPrefabStage.prefabContentsRoot
                : isPrefabAssetObject
                    ? gameObject.transform.root.gameObject
                    : null;

            if (prefabInstanceRoot)
                return isPrefabAssetObject
                    ? new ContextData
                    (
                        type: ContextType.PrefabInstance,
                        key: prefabInstanceRoot.GetInstanceID(),
                        containerType: ContextType.PrefabAsset,
                        containerKey: prefabAssetRoot.GetInstanceID()
                    ) // Prefab instance inside prefab asset
                    : new ContextData
                    (
                        type: ContextType.PrefabInstance,
                        key: prefabInstanceRoot.GetInstanceID(),
                        containerType: ContextType.SceneObject,
                        containerKey: gameObject.scene.handle
                    ); // Prefab instance inside scene

            if (isPrefabAssetObject)
                return new ContextData
                (
                    type: ContextType.PrefabAsset,
                    key: prefabAssetRoot.GetInstanceID(),
                    containerType: ContextType.PrefabAsset,
                    containerKey: prefabAssetRoot.GetInstanceID()
                ); // Prefab asset

            return new ContextData(
                type: ContextType.SceneObject,
                key: gameObject.scene.handle,
                containerType: ContextType.SceneObject,
                containerKey: gameObject.scene.handle
            ); // Scene object
        }

        private class ContextData
        {
            public ContextData(
                ContextType type,
                int key,
                ContextType containerType,
                int containerKey)
            {
                Type = type;
                Key = key;
                ContainerType = containerType;
                ContainerKey = containerKey;
            }

            public ContextType Type { get; }
            public int Key { get; }
            public ContextType ContainerType { get; }
            public int ContainerKey { get; }
        }

        #region Equality logic

        public bool Equals(ContextIdentity other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Type == other.Type && Id == other.Id;
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