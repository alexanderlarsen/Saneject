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
        /// Traverses the <see cref="Scope.ParentScope" /> chain and returns the root scope.
        /// </summary>
        /// <param name="startScope">The scope to start from.</param>
        /// <returns>The root-most parent scope.</returns>
        public static Scope FindRootScope(this Scope startScope)
        {
            Scope rootScope = startScope;

            while (rootScope.ParentScope)
                rootScope = rootScope.ParentScope;

            return rootScope;
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
        /// Sets parent scopes and calls <see cref="Scope.ConfigureBindings" /> on each scope in the array.
        /// </summary>
        /// <param name="scopes">The array of scopes to configure.</param>
        public static void Initialize(this Scope[] scopes)
        {
            foreach (Scope scope in scopes)
            {
                scope.SetParentScope();
                scope.Initialize();
            }
        }

        /// <summary>
        /// Set ParentScope by traversing up the transform hierarchy.
        /// </summary>
        private static void SetParentScope(this Scope scope)
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