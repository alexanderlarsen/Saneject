using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    public class NewComponentFilterBuilder<TComponent> where TComponent : class
    {
        private readonly NewBinding binding;

        public NewComponentFilterBuilder(NewBinding binding)
        {
            this.binding = binding;
        }

        #region BASE METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TComponent" /> for custom search logic.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> Where(Func<TComponent, bool> predicate)
        {
            binding.AddFilter(o => o is TComponent t && predicate(t));
            return this;
        }

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TComponent" /> as <see cref="Component" /> for custom search logic. Useful if <typeparamref name="TComponent" /> is an interface.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereComponent(Func<Component, bool> predicate)
        {
            Where(t => t is Component c && c && predicate(c));
            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s own <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereTransform(Func<Transform, bool> predicate)
        {
            Where(t => t is Component c && c && predicate(c.transform));
            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s own <see cref="GameObject" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereGameObject(Func<GameObject, bool> predicate)
        {
            Where(o => o is Component c && c && predicate(c.gameObject));
            return this;
        }

        #endregion

        #region PARENT / ANCESTOR METHODS

        /// <summary>
        /// Filter using a predicate on the candidate’s immediate parent <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereParent(Func<Transform, bool> predicate)
        {
            WhereTransform(t => t.parent && predicate(t.parent));
            return this;
        }

        /// <summary>
        /// Filter using a predicate on any ancestor <see cref="Transform" /> of the candidate.
        /// Supports optional inclusion of the candidate itself and limiting the depth.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereAnyAncestor(
            Func<Transform, bool> predicate,
            bool includeSelf = false,
            int maxDepth = int.MaxValue)
        {
            WhereTransform(t =>
            {
                Transform cur = includeSelf ? t : t.parent;
                int depth = 0;

                while (cur && depth++ < maxDepth)
                {
                    if (predicate(cur)) return true;
                    cur = cur.parent;
                }

                return false;
            });

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s root <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereRoot(Func<Transform, bool> predicate)
        {
            WhereTransform(t => t.root && predicate(t.root));
            return this;
        }

        #endregion

        #region CHILD / DESCENDANT METHODS

        /// <summary>
        /// Filter using a predicate on any direct child <see cref="Transform" /> of the candidate.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereAnyChild(Func<Transform, bool> predicate)
        {
            WhereTransform(t =>
            {
                for (int i = 0; i < t.childCount; i++)
                    if (predicate(t.GetChild(i)))
                        return true;

                return false;
            });

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the child at a specific <paramref name="index" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereChildAt(
            int index,
            Func<Transform, bool> predicate)
        {
            WhereTransform(t =>
            {
                if (index < 0 || index >= t.childCount) return false;
                Transform ch = t.GetChild(index);
                return ch && predicate(ch);
            });

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the first direct child of the candidate.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereFirstChild(Func<Transform, bool> predicate)
        {
            WhereChildAt(0, predicate);
            return this;
        }

        /// <summary>
        /// Filter using a predicate on the last direct child of the candidate.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereLastChild(Func<Transform, bool> predicate)
        {
            WhereTransform(t =>
            {
                if (t.childCount == 0) return false;
                Transform ch = t.GetChild(t.childCount - 1);
                return ch && predicate(ch);
            });

            return this;
        }

        /// <summary>
        /// Filter using a predicate on any descendant <see cref="Transform" /> of the candidate (recursive search).
        /// Supports optional inclusion of the candidate itself.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereAnyDescendant(
            Func<Transform, bool> predicate,
            bool includeSelf = false)
        {
            WhereTransform(t =>
            {
                Stack<Transform> stack = new();
                if (includeSelf) stack.Push(t);
                for (int i = 0; i < t.childCount; i++) stack.Push(t.GetChild(i));

                while (stack.Count > 0)
                {
                    Transform cur = stack.Pop();
                    if (cur && predicate(cur)) return true;

                    for (int i = 0; i < cur.childCount; i++)
                        stack.Push(cur.GetChild(i));
                }

                return false;
            });

            return this;
        }

        /// <summary>
        /// Filter using a predicate on any sibling <see cref="Transform" /> of the candidate (excluding itself).
        /// </summary>
        public NewComponentFilterBuilder<TComponent> WhereAnySibling(Func<Transform, bool> predicate)
        {
            WhereTransform(t =>
            {
                if (!t.parent) return false;
                Transform p = t.parent;

                for (int i = 0; i < p.childCount; i++)
                {
                    Transform s = p.GetChild(i);
                    if (s == t) continue;
                    if (predicate(s)) return true;
                }

                return false;
            });

            return this;
        }

        #endregion
    }
}