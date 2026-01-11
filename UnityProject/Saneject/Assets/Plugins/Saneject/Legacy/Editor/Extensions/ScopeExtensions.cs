using Plugins.Saneject.Legacy.Editor.Core;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.Extensions
{
    /// <summary>
    /// Editor-only extension methods for working with <see cref="Scope" /> arrays and hierarchies.
    /// </summary>
    public static class ScopeExtensions
    {
        /// <summary>
        /// Walks up to the root-most <see cref="Scope" />. If the start scope lives on a prefab instance,
        /// the walk stops at that prefab instance root (does not cross into the scene root).
        /// </summary>
        public static Scope FindRootScope(this Scope startingScope)
        {
            Transform currentTransform = startingScope.transform.parent;
            Scope rootScope = startingScope;

            while (currentTransform)
            {
                if (currentTransform.TryGetComponent(out Scope foundScope) && ContextFilter.AreSameContext(startingScope, foundScope))
                    rootScope = foundScope;

                currentTransform = currentTransform.parent;
            }

            return rootScope;
        }

        /// <summary>
        /// Calls <see cref="Scope.Dispose" /> on each scope in the array.
        /// </summary>
        /// <param name="scopes">The array of scopes to dispose.</param>
        public static void DisposeAll(this Scope[] scopes)
        {
            foreach (Scope scope in scopes)
                scope.Dispose();
        }

        /// <summary>
        /// Set ParentScope by traversing up the transform hierarchy.
        /// </summary>
        public static void SetParentScope(this Scope startingScope)
        {
            Transform current = startingScope.transform.parent;

            while (current)
            {
                if (current.TryGetComponent(out Scope foundScope) && ContextFilter.AreSameContext(startingScope, foundScope))
                {
                    startingScope.ParentScope = foundScope;
                    break;
                }

                current = current.parent;
            }
        }
    }
}