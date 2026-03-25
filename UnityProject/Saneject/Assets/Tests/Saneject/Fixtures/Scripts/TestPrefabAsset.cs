using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Saneject.Fixtures.Scripts
{
    public sealed class TestPrefabAsset
    {
        private readonly string rootPath;
        private readonly int width;
        private readonly int depth;
        private readonly Dictionary<string, Transform> transformCache = new();
        private readonly Dictionary<(string path, Type type), Component> componentCache = new();

        private TestPrefabAsset(
            string rootPath,
            int width,
            int depth)
        {
            this.rootPath = rootPath;
            this.width = width;
            this.depth = depth;
            Root = new GameObject(rootPath);
            AssetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Tests/Saneject/Fixtures/{rootPath}.prefab");
            transformCache[rootPath] = Root.transform;
        }

        public GameObject Root { get; }

        public string AssetPath { get; }

        public static TestPrefabAsset Create(
            string rootName,
            int width,
            int depth)
        {
            if (string.IsNullOrWhiteSpace(rootName))
                throw new ArgumentException("Root name must not be null or whitespace.", nameof(rootName));

            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");

            if (depth < 1)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be at least 1.");

            TestPrefabAsset prefab = new(rootName, width, depth);
            prefab.CreateHierarchy();
            return prefab;
        }

        public static void CloseStage()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
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

        public TestPrefabInstance Instantiate(
            Transform parent,
            string name)
        {
            GameObject prefabAsset = Save();
            GameObject prefabInstanceRoot = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;

            if (!prefabInstanceRoot)
                throw new InvalidOperationException($"Failed to instantiate prefab asset: {AssetPath}");

            if (parent)
                prefabInstanceRoot.transform.SetParent(parent, false);

            if (!string.IsNullOrEmpty(name))
                prefabInstanceRoot.name = name;

            return new TestPrefabInstance(rootPath, AssetPath, prefabInstanceRoot);
        }

        public TestPrefabInstance OpenStage()
        {
            Save();
            PrefabStage prefabStage = PrefabStageUtility.OpenPrefab(AssetPath);

            if (prefabStage == null)
                throw new InvalidOperationException($"Failed to open prefab stage for asset: {AssetPath}");

            if (!prefabStage.prefabContentsRoot)
                throw new InvalidOperationException($"Prefab stage did not contain a root object for asset: {AssetPath}");

            return new TestPrefabInstance(rootPath, AssetPath, prefabStage.prefabContentsRoot);
        }

        public void Destroy()
        {
            if (Root)
                Object.DestroyImmediate(Root);
        }

        public bool DeleteAsset()
        {
            return AssetDatabase.DeleteAsset(AssetPath);
        }

        private GameObject Save()
        {
            PrefabUtility.SaveAsPrefabAsset(Root, AssetPath);
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);

            return prefabAsset
                ? prefabAsset
                : throw new InvalidOperationException($"Failed to save prefab asset: {AssetPath}");
        }

        private void CreateHierarchy()
        {
            CreateChildren(Root.transform, rootPath, currentDepth: 1);
        }

        private void CreateChildren(
            Transform parent,
            string parentPath,
            int currentDepth)
        {
            if (currentDepth >= depth)
                return;

            for (int childIndex = 1; childIndex <= width; childIndex++)
            {
                string childPath = $"{parentPath}/Child {childIndex}";
                GameObject child = new($"Child {childIndex}");
                child.transform.SetParent(parent, false);
                transformCache[childPath] = child.transform;
                CreateChildren(child.transform, childPath, currentDepth + 1);
            }
        }
    }
}
