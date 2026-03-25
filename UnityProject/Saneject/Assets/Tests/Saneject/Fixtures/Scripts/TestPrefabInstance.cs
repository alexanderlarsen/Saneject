using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Fixtures.Scripts
{
    public sealed class TestPrefabInstance
    {
        private readonly string rootPath;
        private readonly Dictionary<string, Transform> transformCache = new();
        private readonly Dictionary<(string path, Type type), Component> componentCache = new();

        internal TestPrefabInstance(
            string rootPath,
            string assetPath,
            GameObject root)
        {
            this.rootPath = rootPath;
            AssetPath = assetPath;
            Root = root;
            ValidateRoot();
            CacheHierarchy(Root.transform, rootPath);
        }

        public GameObject Root { get; }

        public string AssetPath { get; }

        public T Get<T>(string path) where T : Component
        {
            (string path, Type type) key = (path, typeof(T));

            if (componentCache.TryGetValue(key, out Component cachedComponent))
                return cachedComponent as T;

            Transform transform = GetTransform(path);
            T component = transform.GetComponent<T>();

            if (!component)
                throw new InvalidOperationException($"Component '{typeof(T).Name}' not found at path: {path}");

            componentCache[key] = component;
            return component;
        }

        public Transform GetTransform(string path)
        {
            return transformCache.TryGetValue(path, out Transform transform)
                ? transform
                : throw new InvalidOperationException($"Transform not found at path: {path}");
        }

        public T Add<T>(string path) where T : Component
        {
            Transform transform = GetTransform(path);
            T component = transform.gameObject.AddComponent<T>();
            componentCache[(path, typeof(T))] = component;
            return component;
        }

        public Transform AddTransform(string path)
        {
            if (transformCache.ContainsKey(path))
                throw new InvalidOperationException($"Transform already exists at path: {path}");

            int separatorIndex = path.LastIndexOf('/');

            if (separatorIndex < 0)
                throw new InvalidOperationException($"Transform path must include a parent path: {path}");

            string parentPath = path[..separatorIndex];
            string name = path[(separatorIndex + 1)..];
            Transform parentTransform = GetTransform(parentPath);
            GameObject child = new(name);
            child.transform.SetParent(parentTransform, false);
            transformCache[path] = child.transform;
            return child.transform;
        }

        public T[] AddToAllTransforms<T>() where T : Component
        {
            List<T> components = new();

            foreach ((string path, Transform _) in transformCache)
                components.Add(Add<T>(path));

            return components.ToArray();
        }

        public T[] AddToRoots<T>() where T : Component
        {
            return new[] { Add<T>(rootPath) };
        }

        public T[] AddToLeafs<T>() where T : Component
        {
            return transformCache
                .Where(pair => pair.Value.childCount == 0)
                .Select(pair => Add<T>(pair.Key))
                .ToArray();
        }

        public void Destroy()
        {
            if (Root)
                Object.DestroyImmediate(Root);
        }

        private void ValidateRoot()
        {
            if (PrefabUtility.IsPartOfPrefabInstance(Root))
            {
                string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Root);

                if (string.Equals(prefabAssetPath, AssetPath, StringComparison.Ordinal))
                    return;
            }

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null &&
                prefabStage.prefabContentsRoot == Root &&
                string.Equals(prefabStage.assetPath, AssetPath, StringComparison.Ordinal))
            {
                return;
            }

            throw new InvalidOperationException($"GameObject '{Root.name}' is not an instance of prefab asset: {AssetPath}");
        }

        private void CacheHierarchy(
            Transform parent,
            string parentPath)
        {
            if (!parentPath.StartsWith(rootPath, StringComparison.Ordinal))
                throw new InvalidOperationException($"Transform path is not part of prefab root '{rootPath}': {parentPath}");

            transformCache[parentPath] = parent;

            foreach (Transform child in parent)
                CacheHierarchy(child, $"{parentPath}/{child.name}");
        }
    }
}
