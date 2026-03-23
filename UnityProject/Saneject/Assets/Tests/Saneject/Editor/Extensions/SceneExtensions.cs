using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Saneject.Editor.Extensions
{
    public static class SceneExtensions
    {
        public static T AddComponentAtPath<T>(
            this Scene scene,
            string path) where T : Component
        {
            return scene.GetTransformAtPath(path).gameObject.AddComponent<T>();
        }

        public static T GetComponentAtPath<T>(
            this Scene scene,
            string path) where T : Component
        {
            return scene.GetTransformAtPath(path).GetComponent<T>();
        }

        public static Transform GetTransformAtPath(
            this Scene scene,
            string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            string[] parts = path.Split('/');
            string rootName = parts[0];

            IEnumerable<GameObject> rootObjects = scene
                .GetRootGameObjects()
                .Where(rootGameObject => rootGameObject.name == rootName);

            foreach (GameObject rootGameObject in rootObjects)
            {
                if (parts.Length == 1)
                    return rootGameObject.transform;

                string relativePath = string.Join("/", parts, 1, parts.Length - 1);
                Transform targetTransform = rootGameObject.transform.Find(relativePath);

                if (targetTransform)
                    return targetTransform;
            }

            throw new InvalidOperationException($"Transform not found at path: {path}");
        }
    }
}