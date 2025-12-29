using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeParent()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Parent);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeAncestors(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Ancestors);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeSiblings()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Siblings);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region ROOT LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootSelf()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
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
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetParent()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Parent);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetAncestors(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Ancestors);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetSiblings()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Siblings);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region CUSTOM TARGET LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> From(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Self);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromParentOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Parent);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
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
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Ancestors);
            binding.SetCustomTargetTransform(target);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromFirstChildOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.FirstChild);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromLastChildOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.LastChild);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.ChildAtIndex);
            binding.SetCustomTargetTransform(target);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromDescendantsOf(
            Transform target,
            bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Descendants);
            binding.SetCustomTargetTransform(target);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromSiblingsOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Siblings);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region SPECIAL LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> anywhere in the scene using <see cref="UnityEngine.Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromAnywhereInScene(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            binding.SetSearchParameters(SearchOrigin.Scene, SearchDirection.Anywhere);
            binding.SetSceneSearchFindObjectsSettings(findObjectsInactive, sortMode);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromInstance(TComponent instance)
        {
            binding.SetSearchParameters(SearchOrigin.SingleInstance, SearchDirection.None);
            binding.ResolveFromInstances(instance);
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<TComponent> method)
        {
            binding.SetSearchParameters(SearchOrigin.SingleInstance, SearchDirection.None);
            binding.ResolveFromInstances(method?.Invoke());
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Component" />s using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<IEnumerable<TComponent>> method)
        {
            binding.SetSearchParameters(SearchOrigin.MultipleInstances, SearchDirection.None);
            binding.ResolveFromInstances(method?.Invoke().Cast<UnityEngine.Component>());
            binding.MarkLocatorStrategySpecified();
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion
    }
}