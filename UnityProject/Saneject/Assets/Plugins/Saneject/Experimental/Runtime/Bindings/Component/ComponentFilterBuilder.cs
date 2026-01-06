using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public class ComponentFilterBuilder<TComponent> where TComponent : class
    {
        private readonly ComponentBinding binding;

        public ComponentFilterBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        #region BASE METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TComponent" /> for custom search logic.
        /// </summary>
        public ComponentFilterBuilder<TComponent> Where(Func<TComponent, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.Where,
                    o => o is TComponent t && predicate(t)
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TComponent" /> as <see cref="Component" /> for custom search logic. Useful if <typeparamref name="TComponent" /> is an interface.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereComponent(Func<UnityEngine.Component, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereComponent,
                    t => t is UnityEngine.Component c && c && predicate(c)
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s own <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereTransform(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereTransform,
                    t => t is UnityEngine.Component c && c && predicate(c.transform)
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s own <see cref="GameObject" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereGameObject(Func<GameObject, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereGameObject,
                    o => o is UnityEngine.Component c && c && predicate(c.gameObject)
                ));

            return this;
        }

        #endregion

        #region PARENT / ANCESTOR METHODS

        /// <summary>
        /// Filter using a predicate on the candidate’s immediate parent <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereParent(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereParent,
                    t => t is UnityEngine.Component c && c && predicate(c.transform.parent)
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on any ancestor <see cref="Transform" /> of the candidate.
        /// Supports optional inclusion of the candidate itself and limiting the depth.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereAnyAncestor(
            Func<Transform, bool> predicate,
            bool includeSelf = false,
            int maxDepth = int.MaxValue)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereAnyAncestor,
                    o =>
                    {
                        if (o is not UnityEngine.Component c || !c)
                            return false;

                        Transform cur = includeSelf ? c.transform : c.transform.parent;
                        int depth = 0;

                        while (cur && depth++ < maxDepth)
                        {
                            if (predicate(cur))
                                return true;

                            cur = cur.parent;
                        }

                        return false;
                    }
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the candidate’s root <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereRoot(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereRoot,
                    t => t is UnityEngine.Component c && c && c.transform.root && predicate(c.transform.root)
                ));

            return this;
        }

        #endregion

        #region CHILD / DESCENDANT METHODS

        /// <summary>
        /// Filter using a predicate on any direct child <see cref="Transform" /> of the candidate.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereAnyChild(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereAnyChild,
                    t =>
                    {
                        if (t is not UnityEngine.Component c || !c)
                            return false;

                        Transform tr = c.transform;

                        for (int i = 0; i < tr.childCount; i++)
                            if (predicate(tr.GetChild(i)))
                                return true;

                        return false;
                    }
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the child at a specific <paramref name="index" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereChildAt(
            int index,
            Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereChildAt,
                    t =>
                    {
                        if (t is not UnityEngine.Component c || !c)
                            return false;

                        Transform tr = c.transform;

                        if (index < 0 || index >= tr.childCount)
                            return false;

                        Transform ch = tr.GetChild(index);
                        return ch && predicate(ch);
                    }
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the first direct child of the candidate.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereFirstChild(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereFirstChild,
                    t =>
                        t is UnityEngine.Component c &&
                        c &&
                        c.transform.childCount > 0 &&
                        predicate(c.transform.GetChild(0))
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on the last direct child of the candidate.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereLastChild(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereLastChild,
                    t =>
                    {
                        if (t is not UnityEngine.Component c || !c)
                            return false;

                        Transform tr = c.transform;

                        if (tr.childCount == 0)
                            return false;

                        Transform ch = tr.GetChild(tr.childCount - 1);
                        return ch && predicate(ch);
                    }
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on any descendant <see cref="Transform" /> of the candidate (recursive search).
        /// Supports optional inclusion of the candidate itself.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereAnyDescendant(
            Func<Transform, bool> predicate,
            bool includeSelf = false)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereAnyDescendant,
                    t =>
                    {
                        if (t is not UnityEngine.Component c || !c)
                            return false;

                        Transform tr = c.transform;
                        Stack<Transform> stack = new();

                        if (includeSelf)
                            stack.Push(tr);

                        for (int i = 0; i < tr.childCount; i++)
                            stack.Push(tr.GetChild(i));

                        while (stack.Count > 0)
                        {
                            Transform cur = stack.Pop();

                            if (cur && predicate(cur))
                                return true;

                            for (int i = 0; i < cur.childCount; i++)
                                stack.Push(cur.GetChild(i));
                        }

                        return false;
                    }
                ));

            return this;
        }

        /// <summary>
        /// Filter using a predicate on any sibling <see cref="Transform" /> of the candidate (excluding itself).
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereAnySibling(Func<Transform, bool> predicate)
        {
            binding.DependencyFilters.Add(
                new ComponentDependencyFilter
                (
                    ComponentFilterType.WhereAnySibling,
                    t =>
                    {
                        if (t is not UnityEngine.Component c || !c)
                            return false;

                        Transform tr = c.transform;
                        Transform parent = tr.parent;

                        if (!parent)
                            return false;

                        for (int i = 0; i < parent.childCount; i++)
                        {
                            Transform sibling = parent.GetChild(i);

                            if (sibling == tr)
                                continue;

                            if (predicate(sibling))
                                return true;
                        }

                        return false;
                    }
                ));

            return this;
        }

        #endregion
    }
}