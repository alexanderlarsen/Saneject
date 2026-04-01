using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings.Component
{
    /// <summary>
    /// Base class for component binding builders, providing fluent methods for locating components.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component being bound. Can also be an interface.</typeparam>
    public abstract class BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        /// <summary>
        /// The <see cref="ComponentBinding" /> being configured.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected readonly ComponentBinding binding;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponentBindingBuilder{TComponent}" /> class.
        /// </summary>
        /// <param name="binding">The <see cref="ComponentBinding" /> to configure.</param>
        protected BaseComponentBindingBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        #region DEFAULT (SCOPE) LOCATOR METHODS

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeSelf" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromSelf()
        {
            return FromScopeSelf();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// Shorthand for <see cref="FromScopeParent" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromParent()
        {
            return FromScopeParent();
        }

        /// <summary>
        /// Locate the component on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeAncestors" />.
        /// </summary>
        /// <param name="includeSelf">Whether to include the <see cref="Scope" />'s own transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromAncestors(bool includeSelf = false)
        {
            return FromScopeAncestors(includeSelf);
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeFirstChild" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromFirstChild()
        {
            return FromScopeFirstChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeLastChild" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromLastChild()
        {
            return FromScopeLastChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// Shorthand for <see cref="FromScopeChildWithIndex" />.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromChildWithIndex(int index)
        {
            return FromScopeChildWithIndex(index);
        }

        /// <summary>
        /// Locate the component on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeDescendants" />.
        /// </summary>
        /// <param name="includeSelf">Whether to include the <see cref="Scope" />'s own transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromDescendants(bool includeSelf = false)
        {
            return FromScopeDescendants(includeSelf);
        }

        /// <summary>
        /// Locate the component on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// Shorthand for <see cref="FromScopeSiblings" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromSiblings()
        {
            return FromScopeSiblings();
        }

        #endregion

        #region SCOPE LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeSelf()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeParent()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Parent;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        /// <param name="includeSelf">Whether to include the current transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeAncestors(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Ancestors;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeLastChild()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        /// <param name="includeSelf">Whether to include the current transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeDescendants(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromScopeSiblings()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Siblings;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region ROOT LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromRootSelf()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromRootFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromRootLastChild()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromRootChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        /// <param name="includeSelf">Whether to include the root in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromRootDescendants(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region INJECTION TARGET LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetSelf()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetParent()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Parent;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="includeSelf">Whether to include the target transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetAncestors(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Ancestors;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetLastChild()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="includeSelf">Whether to include the target transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetDescendants(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the <see cref="UnityEngine.Component" /> of a field/property marked with <see cref="Attributes.InjectAttribute" />.
        /// </summary>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromTargetSiblings()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Siblings;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region CUSTOM TRANSFORM LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        /// <param name="transform">The transform to search on.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> From(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Self;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        /// <param name="transform">The transform whose parent to search on.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromParentOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Parent;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        /// <param name="target">The transform whose ancestors to search.</param>
        /// <param name="includeSelf">Whether to include the target transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromAncestorsOf(
            Transform target,
            bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Ancestors;
            binding.CustomTargetTransform = target;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        /// <param name="transform">The transform whose first child to search on.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromFirstChildOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        /// <param name="transform">The transform whose last child to search on.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromLastChildOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        /// <param name="transform">The transform whose child to search on.</param>
        /// <param name="index">The child index.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromChildWithIndexOf(
            Transform transform,
            int index)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.CustomTargetTransform = transform;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        /// <param name="transform">The transform whose descendants to search.</param>
        /// <param name="includeSelf">Whether to include the target transform in the search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromDescendantsOf(
            Transform transform,
            bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.CustomTargetTransform = transform;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        /// <param name="transform">The transform whose siblings to search.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromSiblingsOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Siblings;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region SPECIAL LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> anywhere in the scene using <see cref="UnityEngine.Object.FindObjectsByType(System.Type,UnityEngine.FindObjectsSortMode)" />
        /// - or within an isolated prefab asset using <see cref="UnityEngine.Component.GetComponentsInChildren(System.Type,bool)" /> from the prefab root.
        /// </summary>
        /// <param name="findObjectsInactive">Whether to include inactive objects in the search.</param>
        /// <param name="sortMode">The sort mode for the results.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromAnywhere(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            binding.SearchOrigin = SearchOrigin.Scene;
            binding.SearchDirection = SearchDirection.Anywhere;
            binding.FindObjectsInactive = findObjectsInactive;
            binding.FindObjectsSortMode = sortMode;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="UnityEngine.Component" /> instance.
        /// </summary>
        /// <param name="instance">The component instance.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromInstance(TComponent instance)
        {
            binding.SearchOrigin = SearchOrigin.Instance;
            binding.ResolveFromInstances.Add(instance as Object); // TODO: Silent fail if not UnityEngine.Object?
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="UnityEngine.Component" /> using the provided method.
        /// </summary>
        /// <param name="method">The method to resolve the component.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<TComponent> method)
        {
            binding.SearchOrigin = SearchOrigin.Instance;

            try
            {
                binding.ResolveFromInstances.Add(method?.Invoke() as Object); // TODO: Silent fail if not UnityEngine.Object?
            }
            catch (Exception e)
            {
                binding.FromMethodException = e;
            }

            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="UnityEngine.Component" />s using the provided method.
        /// </summary>
        /// <param name="method">The method to resolve the components.</param>
        /// <returns>A <see cref="ComponentFilterBuilder{TComponent}" /> to further configure the binding.</returns>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<IEnumerable<TComponent>> method)
        {
            binding.SearchOrigin = SearchOrigin.Instance;

            try
            {
                binding.ResolveFromInstances.AddRange(method?.Invoke().OfType<Object>() ?? Enumerable.Empty<Object>()); // TODO: Silent fail if not UnityEngine.Object?
            }
            catch (Exception e)
            {
                binding.FromMethodException = e;
            }

            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion
    }
}