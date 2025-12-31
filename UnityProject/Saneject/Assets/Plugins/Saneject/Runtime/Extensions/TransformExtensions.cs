using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Transform" /> providing utility search method for parent hierarchies.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Enumerate all <typeparamref name="T" /> components on parent <see cref="Transform" />s, optionally including the starting <see cref="Transform" />.
        /// </summary>
        /// <typeparam name="T">The <see cref="Component" /> type to search for.</typeparam>
        /// <param name="transform">The <see cref="Transform" /> to start searching from.</param>
        /// <param name="includeSelf">If true, includes the <see cref="Component" /> on the starting <see cref="Transform" /> itself.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> of matching components found in parents (and optionally self), ordered from nearest to furthest ancestor.</returns>
        public static IEnumerable<T> GetComponentsInParents<T>(
            this Transform transform,
            bool includeSelf = false) where T : class
        {
            if (includeSelf)
            {
                T selfComponent = transform.GetComponent<T>();

                if (selfComponent != null)
                    yield return selfComponent;
            }

            Transform current = transform.parent;

            while (current)
            {
                T component = current.GetComponent<T>();

                if (component != null)
                    yield return component;

                current = current.parent;
            }
        }

     
    }
}