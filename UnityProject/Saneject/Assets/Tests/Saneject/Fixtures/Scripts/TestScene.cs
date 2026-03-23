using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

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

        public static TestScene Create(
            int roots,
            int width,
            int depth)
        {
            if (roots < 1)
                throw new ArgumentOutOfRangeException(nameof(roots), "Root count must be at least 1.");

            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 1.");

            if (depth < 1)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be at least 1.");

            TestScene scene = new(roots, width, depth);
            scene.CreateHierarchy();
            return scene;
        }

        public void AddDependenciesToAllTransforms()
        {
            foreach ((string path, Transform transform) in transformCache)
                AddDependencyIfMissing(path, transform.gameObject);
        }

        public void AddDependenciesToRoots()
        {
            foreach (GameObject root in roots)
                AddDependencyIfMissing(root.name, root);
        }

        public void AddDependenciesToLeafs()
        {
            foreach ((string path, Transform transform) in transformCache)
                if (transform.childCount == 0)
                    AddDependencyIfMissing(path, transform.gameObject);
        }

        public void AddDependencyAt(string path)
        {
            Transform transform = GetTransform(path);
            AddDependencyIfMissing(path, transform.gameObject);
        }

        public T AddComponent<T>(string path) where T : Component
        {
            Transform transform = GetTransform(path);
            T component = transform.gameObject.AddComponent<T>();
            componentCache[(path, typeof(T))] = component;
            return component;
        }

        public T GetComponent<T>(string path) where T : Component
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
            if (!transformCache.TryGetValue(path, out Transform transform))
                throw new InvalidOperationException($"Transform not found at path: {path}");

            return transform;
        }

        private void CreateHierarchy()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

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

        private void AddDependencyIfMissing(
            string path,
            GameObject gameObject)
        {
            if (componentCache.ContainsKey((path, typeof(ComponentDependency))))
                return;

            ComponentDependency dependency = gameObject.GetComponent<ComponentDependency>();

            if (!dependency)
                dependency = gameObject.AddComponent<ComponentDependency>();

            componentCache[(path, typeof(ComponentDependency))] = dependency;
        }
    }
}