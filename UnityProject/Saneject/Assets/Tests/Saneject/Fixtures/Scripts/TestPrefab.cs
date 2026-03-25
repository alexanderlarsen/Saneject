using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tests.Saneject.Fixtures.Scripts
{
    public sealed class TestPrefab
    {
        private readonly string rootPath;
        private readonly Dictionary<string, Transform> transformCache = new();
        private readonly Dictionary<(string path, Type type), Component> componentCache = new();
        private readonly GameObject root;
        private readonly string assetPath;

        private TestPrefab(string rootPath)
        {
            this.rootPath = rootPath;
            root = new(rootPath);
            assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Tests/Saneject/Fixtures/{rootPath}.prefab");
            transformCache[rootPath] = root.transform;
        }

        public GameObject Root => root;

        public string AssetPath => assetPath;

        public static TestPrefab Create(string rootName)
        {
            if (string.IsNullOrWhiteSpace(rootName))
                throw new ArgumentException("Root name must not be null or whitespace.", nameof(rootName));

            return new TestPrefab(rootName);
        }

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

        public T Get<T>(
            Transform rootTransform,
            string path) where T : Component
        {
            Transform transform = GetTransform(rootTransform, path);
            T component = transform.GetComponent<T>();

            if (!component)
                throw new InvalidOperationException($"Component '{typeof(T).Name}' not found at path: {path}");

            return component;
        }

        public Transform GetTransform(string path)
        {
            return transformCache.TryGetValue(path, out Transform transform)
                ? transform
                : throw new InvalidOperationException($"Transform not found at path: {path}");
        }

        public Transform GetTransform(
            Transform rootTransform,
            string path)
        {
            if (!rootTransform)
                throw new ArgumentNullException(nameof(rootTransform));

            if (!transformCache.ContainsKey(path))
                throw new InvalidOperationException($"Transform not found at path: {path}");

            string relativePath = GetRelativePath(path);

            if (string.IsNullOrEmpty(relativePath))
                return rootTransform;

            Transform transform = rootTransform.Find(relativePath);

            return transform
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

        public T[] AddToLeafs<T>() where T : Component
        {
            return transformCache
                .Where(pair => pair.Value.childCount == 0)
                .Select(pair => Add<T>(pair.Key))
                .ToArray();
        }

        public GameObject Instantiate(
            Transform parent,
            string name)
        {
            GameObject prefabAsset = Save();
            GameObject prefabInstanceRoot = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;

            if (!prefabInstanceRoot)
                throw new InvalidOperationException($"Failed to instantiate prefab asset: {assetPath}");

            if (parent)
                prefabInstanceRoot.transform.SetParent(parent, false);

            if (!string.IsNullOrEmpty(name))
                prefabInstanceRoot.name = name;

            return prefabInstanceRoot;
        }

        public PrefabStage OpenStage()
        {
            Save();
            PrefabStage prefabStage = PrefabStageUtility.OpenPrefab(assetPath);

            if (prefabStage == null)
                throw new InvalidOperationException($"Failed to open prefab stage for asset: {assetPath}");

            if (!prefabStage.prefabContentsRoot)
                throw new InvalidOperationException($"Prefab stage did not contain a root object for asset: {assetPath}");

            return prefabStage;
        }

        public void Destroy()
        {
            if (root)
                UnityEngine.Object.DestroyImmediate(root);
        }

        public bool DeleteAsset()
        {
            return AssetDatabase.DeleteAsset(assetPath);
        }

        public static void CloseStage()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        private GameObject Save()
        {
            PrefabUtility.SaveAsPrefabAsset(root, assetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            return prefabAsset
                ? prefabAsset
                : throw new InvalidOperationException($"Failed to save prefab asset: {assetPath}");
        }

        private string GetRelativePath(string path)
        {
            if (path == rootPath)
                return string.Empty;

            string prefix = $"{rootPath}/";

            if (!path.StartsWith(prefix, StringComparison.Ordinal))
                throw new InvalidOperationException($"Transform path is not part of prefab root '{rootPath}': {path}");

            return path[prefix.Length..];
        }
    }
}
