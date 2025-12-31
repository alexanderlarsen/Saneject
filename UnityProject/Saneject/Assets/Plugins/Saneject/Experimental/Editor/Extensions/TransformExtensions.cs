using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class TransformExtensions
    {
        public static IEnumerable<Component> GetComponentsInAncestors(
            this Transform transform,
            Type type,
            bool includeSelf = false)
        {
            if (includeSelf)
            {
                Component selfComponent = transform.GetComponent(type);

                if (selfComponent != null)
                    yield return selfComponent;
            }

            Transform current = transform.parent;

            while (current)
            {
                Component component = current.GetComponent(type);

                if (component != null)
                    yield return component;

                current = current.parent;
            }
        }

        public static IEnumerable<Component> GetComponentsInDescendants(
            this Transform transform,
            Type type,
            bool includeSelf = false)
        {
            return includeSelf
                ? transform.GetComponentsInChildren(type, true)
                : transform.GetComponentsInChildren(type, true).Where(c => c.transform != transform);
        }

        public static IEnumerable<Component> GetComponentsInSiblings(
            this Transform transform,
            Type type)
        {
            return transform?.parent?
                .Cast<Transform>()
                .Where(siblingTransform => siblingTransform != transform)
                .SelectMany(siblingTransform => siblingTransform.GetComponents(type))
                .Where(component => component != null);
        }
    }
}