using System;
using Plugins.Saneject.Runtime.Bindings;
using UnityEngine;

namespace Tests.Runtime
{
    /// <summary>
    /// Small, opt-in sample of “sugar” extensions built on top of the core
    /// <see cref="ComponentFilterBuilder{TComponent}" /> API. These are examples only;
    /// feel free to copy and adapt for your own project conventions.
    /// </summary>
    public static class ComponentFilterSampleExtensions
    {
        #region SELF (Behaviour)

        /// <summary>
        /// Filter to only search for <see cref="Behaviour" /> components where
        /// <see cref="Behaviour.isActiveAndEnabled" /> is true.
        /// </summary>
        public static ComponentFilterBuilder<TBehaviour> WhereIsActiveAndEnabled<TBehaviour>(this ComponentFilterBuilder<TBehaviour> builder) where TBehaviour : Behaviour
        {
            return builder.Where(b => b.isActiveAndEnabled);
        }

        #endregion

        #region PARENT

        /// <summary>
        /// Filter to only search for candidates whose parent’s <see cref="GameObject.name" />
        /// matches <paramref name="name" />.
        /// </summary>
        public static ComponentFilterBuilder<T> WhereParentNameIs<T>(
            this ComponentFilterBuilder<T> builder,
            string name,
            StringComparison cmp = StringComparison.Ordinal) where T : class
        {
            return builder.WhereParent(p => p.name.Equals(name, cmp));
        }

        #endregion

        #region CHILD

        /// <summary>
        /// Filter to only search for candidates with any direct child whose
        /// <see cref="GameObject.name" /> matches <paramref name="name" />.
        /// </summary>
        public static ComponentFilterBuilder<T> WhereAnyChildNameIs<T>(
            this ComponentFilterBuilder<T> builder,
            string name,
            StringComparison cmp = StringComparison.Ordinal) where T : class
        {
            return builder.WhereAnyChild(c => c.name.Equals(name, cmp));
        }

        #endregion

        #region ANCESTOR

        /// <summary>
        /// Filter to only search for candidates with any ancestor whose
        /// <see cref="GameObject.tag" /> equals <paramref name="tag" />.
        /// </summary>
        public static ComponentFilterBuilder<T> WhereAnyAncestorTagIs<T>(
            this ComponentFilterBuilder<T> builder,
            string tag) where T : class
        {
            return builder.WhereAnyAncestor(a => a.CompareTag(tag));
        }

        #endregion

        #region SIBLING

        /// <summary>
        /// Filter to only search for candidates with any sibling whose
        /// <see cref="GameObject.layer" /> equals <paramref name="layer" />.
        /// </summary>
        public static ComponentFilterBuilder<T> WhereAnySiblingLayerIs<T>(
            this ComponentFilterBuilder<T> builder,
            int layer) where T : class
        {
            return builder.WhereAnySibling(s => s.gameObject.layer == layer);
        }

        #endregion
    }
}