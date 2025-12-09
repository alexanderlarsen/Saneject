using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Editor.Extensions
{
    /// <summary>
    /// Editor-only extension methods for working with <see cref="Scope" /> arrays and hierarchies.
    /// </summary>
    public static class ScopeExtensions
    {
        /// <summary>
        /// Sets parent scopes, configures bindings, and validates them for each scope in the array.
        /// </summary>
        /// <param name="scopes">The array of scopes to configure.</param>
        public static void InitializeScopes(this Scope[] scopes)
        {
            foreach (Scope scope in scopes)
            {
                scope.SetParentScope();
                scope.ConfigureBindings();
                scope.ValidateBindings();
            }
        }

        /// <summary>
        /// Walks up to the root-most <see cref="Scope" />. If the start scope lives on a prefab instance,
        /// the walk stops at that prefab instance root (does not cross into the scene root).
        /// </summary>
        public static Scope FindRootScope(this Scope startScope)
        {
            Scope current = startScope;
            bool clampToPrefab = current.gameObject.IsPrefab();

            while (true)
            {
                Transform parent = current.transform.parent;

                if (!parent)
                    break;

                // If we started inside a prefab, don't cross out of it.
                if (clampToPrefab && !parent.gameObject.IsPrefab())
                    break;

                // Start lookup from the parent, not from 'current' (GetComponentInParent includes self). 
                Scope next = parent.GetComponentInParent<Scope>(true);

                if (!next || next == current)
                    break;

                current = next;
            }

            return current;
        }

        /// <summary>
        /// Calls <see cref="Scope.Dispose" /> on each scope in the array.
        /// </summary>
        /// <param name="scopes">The array of scopes to dispose.</param>
        public static void Dispose(this Scope[] scopes)
        {
            foreach (Scope scope in scopes)
                scope.Dispose();
        }

        /// <summary>
        /// Set ParentScope by traversing up the transform hierarchy.
        /// </summary>
        public static void SetParentScope(this Scope scope)
        {
            Transform current = scope.transform.parent;

            while (current)
            {
                if (current.TryGetComponent(out Scope parentScope))
                {
                    scope.ParentScope = parentScope;
                    break;
                }

                current = current.parent;
            }
        }
    }
}