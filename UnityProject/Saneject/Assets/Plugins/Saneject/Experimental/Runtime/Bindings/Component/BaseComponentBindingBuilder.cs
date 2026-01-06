using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Component
{
    public abstract class BaseComponentBindingBuilder<TComponent> where TComponent : class
    {
        protected readonly ComponentBinding binding;

        protected BaseComponentBindingBuilder(ComponentBinding binding)
        {
            this.binding = binding;
        }

        #region DEFAULT (SCOPE) LOCATOR METHODS

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeSelf" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromSelf()
        {
            return FromScopeSelf();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// Shorthand for <see cref="FromScopeParent" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromParent()
        {
            return FromScopeParent();
        }

        /// <summary>
        /// Locate the component on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeAncestors" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromAncestors(bool includeSelf = false)
        {
            return FromScopeAncestors(includeSelf);
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeFirstChild" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromFirstChild()
        {
            return FromScopeFirstChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeLastChild" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromLastChild()
        {
            return FromScopeLastChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// Shorthand for <see cref="FromScopeChildWithIndex" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromChildWithIndex(int index)
        {
            return FromScopeChildWithIndex(index);
        }

        /// <summary>
        /// Locate the component on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeDescendants" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromDescendants(bool includeSelf = false)
        {
            return FromScopeDescendants(includeSelf);
        }

        /// <summary>
        /// Locate the component on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// Shorthand for <see cref="FromScopeSiblings" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromSiblings()
        {
            return FromScopeSiblings();
        }

        #endregion

        #region SCOPE LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeSelf()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeParent()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Parent;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeAncestors(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Ancestors;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeLastChild()
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeDescendants(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.Scope;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
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
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootSelf()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootLastChild()
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.Root;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
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
        /// Locate the <see cref="Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetSelf()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Self;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetParent()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Parent;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetAncestors(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Ancestors;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetFirstChild()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetLastChild()
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetChildWithIndex(int index)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.ChildAtIndex;
            binding.ChildIndexForSearch = index;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetDescendants(bool includeSelf = false)
        {
            binding.SearchOrigin = SearchOrigin.InjectionTarget;
            binding.SearchDirection = SearchDirection.Descendants;
            binding.IncludeSelfInSearch = includeSelf;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
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
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> From(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Self;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromParentOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.Parent;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
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
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromFirstChildOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.FirstChild;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromLastChildOf(Transform transform)
        {
            binding.SearchOrigin = SearchOrigin.CustomTargetTransform;
            binding.SearchDirection = SearchDirection.LastChild;
            binding.CustomTargetTransform = transform;
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
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
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
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
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
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
        /// Locate the <see cref="Component" /> anywhere in the scene using <see cref="UnityEngine.Object.FindObjectsByType(System.Type,UnityEngine.FindObjectsSortMode)" />
        /// - or within an isolated prefab asset using <see cref="UnityEngine.Component.GetComponentsInChildren(System.Type,bool)" /> from the prefab root.
        /// </summary>
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
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromInstance(TComponent instance)
        {
            binding.SearchOrigin = SearchOrigin.Instance;
            binding.ResolveFromInstances.Add(instance as Object); // TODO: Silent fail if not UnityEngine.Object?
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<TComponent> method)
        {
            binding.SearchOrigin = SearchOrigin.Instance;
            binding.ResolveFromInstances.Add(method?.Invoke() as Object); // TODO: Silent fail if not UnityEngine.Object?
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Component" />s using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<IEnumerable<TComponent>> method)
        {
            binding.SearchOrigin = SearchOrigin.Instance;
            binding.ResolveFromInstances.AddRange(method?.Invoke().OfType<Object>() ?? Enumerable.Empty<Object>()); // TODO: Silent fail if not UnityEngine.Object?
            binding.LocatorStrategySpecified = true;
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion
    }
}