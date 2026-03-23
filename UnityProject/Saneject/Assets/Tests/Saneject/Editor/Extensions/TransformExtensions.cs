using System;
using UnityEngine;

namespace Tests.Saneject.Editor.Extensions
{
    public static class TransformExtensions
    {
        public static T AddComponentAtPath<T>(
            this Transform transform,
            string path) where T : Component
        {
            Transform targetTransform = transform.Find(path);

            return targetTransform == null
                ? throw new InvalidOperationException($"Transform not found at path: {path}")
                : targetTransform.gameObject.AddComponent<T>();
        }

        public static T GetComponentAtPath<T>(
            this Transform transform,
            string path) where T : Component
        {
            Transform targetTransform = transform.Find(path);

            return targetTransform == null
                ? throw new InvalidOperationException($"Transform not found at path: {path}")
                : targetTransform.gameObject.GetComponent<T>();
        }
    }
}