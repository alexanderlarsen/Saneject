using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Saneject.Fixtures.Scripts
{
    public sealed class TestScene
    {
        private readonly int rootCount;
        private readonly int width;
        private readonly int depth;
        private readonly Dictionary<string, Transform> transformCache = new();
        private readonly Dictionary<(string path, Type type), Component> componentCache = new();
        private readonly List<GameObject> roots = new();
        private string scenePath;

        private TestScene(
            int rootCount,
            int width,
            int depth)
        {
            this.rootCount = rootCount;
            this.width = width;
            this.depth = depth;
        }

        public IReadOnlyList<GameObject> Roots => roots;
        public Scene Scene { get; private set; }
        public string ScenePath => scenePath;

        public static TestScene Create(
            int roots,
            int width,
            int depth,
            NewSceneMode newSceneMode = NewSceneMode.Single)
        {
            if (roots < 1)
                throw new ArgumentOutOfRangeException(nameof(roots), "Root count must be at least 1.");

            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");

            if (depth < 1)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be at least 1.");

            TestScene scene = new(roots, width, depth);
            scene.CreateHierarchy(newSceneMode);
            return scene;
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

        public T[] AddToAllTransforms<T>() where T : Component
        {
            List<T> components = new();
            
            foreach ((string path, Transform _) in transformCache)
                components.Add(Add<T>(path));
            
            return components.ToArray();
        }

        public T[] AddToRoots<T>() where T : Component
        {
            return roots
                .Select(root => Add<T>(root.name))
                .ToArray();
        }

        public T[] AddToLeafs<T>() where T : Component
        {
            return transformCache
                .Where(pair => pair.Value.childCount == 0)
                .Select(pair => Add<T>(pair.Key))
                .ToArray();
        }

        public string SaveToDisk()
        {
            scenePath ??= AssetDatabase.GenerateUniqueAssetPath("Assets/Tests/Saneject/Fixtures/TestScene.unity");
            EditorSceneManager.SaveScene(Scene, scenePath);
            return scenePath;
        }

        public bool DeleteFromDisk()
        {
            return !string.IsNullOrEmpty(scenePath) &&
                   AssetDatabase.DeleteAsset(scenePath);
        }

        private void CreateHierarchy(NewSceneMode newSceneMode)
        {
            Scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, newSceneMode);

            for (int rootIndex = 1; rootIndex <= rootCount; rootIndex++)
            {
                string rootPath = $"Root {rootIndex}";
                GameObject root = new(rootPath);
                roots.Add(root);
                transformCache[rootPath] = root.transform;
                CreateChildren(root.transform, rootPath, currentDepth: 1);
            }
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
