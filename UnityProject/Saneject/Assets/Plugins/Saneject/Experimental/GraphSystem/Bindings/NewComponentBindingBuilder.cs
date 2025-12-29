using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Bindings
{
    /// <summary>
    /// Fluent builder for configuring component bindings within a <see cref="Scope" />.
    /// Allows specifying how to locate a <see cref="Component" /> instance (or collection) from the scene hierarchy or injection context.
    /// Supports locating from the scope, injection target, custom transforms, scene-wide queries, or explicit instances.
    /// Typically returned from binding methods like <c>BindComponent&lt;TComponent&gt;()</c> or <c>BindMultipleComponents&lt;TComponent&gt;()</c>.
    /// </summary>
    public class NewComponentBindingBuilder<TComponent> where TComponent : class
    {
        private readonly NewComponentBinding binding;

        public NewComponentBindingBuilder(NewComponentBinding binding)
        {
            this.binding = binding;
        }

        #region QUALIFIER METHODS

        /// <summary>
        /// Qualifies this binding with an ID.
        /// Only injection targets annotated with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />
        /// that specify the same ID will resolve using this binding.
        /// </summary>
        /// <param name="ids">The identifiers to match against injection targets.</param>
        public NewComponentBindingBuilder<TComponent> ToID(params string[] ids)
        {
            foreach (string id in ids)
                binding.AddIdQualifier(id);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        public NewComponentBindingBuilder<TComponent> ToTarget<TTarget>()
        {
            binding.AddInjectionTargetTypeQualifier(typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="UnityEngine.Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        public NewComponentBindingBuilder<TComponent> ToTarget(params Type[] targetTypes)
        {
            foreach (Type type in targetTypes)
                binding.AddInjectionTargetTypeQualifier(type);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property) has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        public NewComponentBindingBuilder<TComponent> ToMember(params string[] memberNames)
        {
            foreach (string memberName in memberNames)
                binding.AddInjectionTargetMemberNameQualifier(memberName);

            return this;
        }

        #endregion

        #region DEFAULT (SCOPE) LOCATOR METHODS

        /// <summary>
        /// Locate the component on the <see cref="NewScope" />'s own <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeSelf" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromSelf()
        {
            return FromScopeSelf();
        }

        /// <summary>
        /// Locate the component on the <see cref="NewScope" />'s direct <see cref="Transform.parent" />.
        /// Shorthand for <see cref="FromScopeParent" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromParent()
        {
            return FromScopeParent();
        }

        /// <summary>
        /// Locate the component on any ancestor <see cref="Transform" /> of the <see cref="NewScope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeAncestors" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromAncestors(bool includeSelf = false)
        {
            return FromScopeAncestors(includeSelf);
        }

        /// <summary>
        /// Locate the component on the <see cref="NewScope" />'s first direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeFirstChild" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromFirstChild()
        {
            return FromScopeFirstChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="NewScope" />'s last direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeLastChild" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromLastChild()
        {
            return FromScopeLastChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="NewScope" />'s direct child <see cref="Transform" /> with index.
        /// Shorthand for <see cref="FromScopeChildWithIndex" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromChildWithIndex(int index)
        {
            return FromScopeChildWithIndex(index);
        }

        /// <summary>
        /// Locate the component on any descendant <see cref="Transform" /> of the <see cref="NewScope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeDescendants" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromDescendants(bool includeSelf = false)
        {
            return FromScopeDescendants(includeSelf);
        }

        /// <summary>
        /// Locate the component on any sibling <see cref="Transform" /> of the <see cref="NewScope" /> (other children of the same parent).
        /// Shorthand for <see cref="FromScopeSiblings" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromSiblings()
        {
            return FromScopeSiblings();
        }

        #endregion

        #region SCOPE LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" />'s own <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeSelf()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeParent()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Parent);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="NewScope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeAncestors(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Ancestors);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="NewScope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="NewScope" /> (other children of the same parent).
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromScopeSiblings()
        {
            binding.SetSearchParameters(SearchOrigin.Scope, SearchDirection.Siblings);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region ROOT LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromRootSelf()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromRootFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromRootLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="NewScope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromRootChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="NewScope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromRootDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.Root, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region INJECTION TARGET LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetSelf()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Self);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetParent()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Parent);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetAncestors(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Ancestors);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetFirstChild()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.FirstChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetLastChild()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.LastChild);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetChildWithIndex(int index)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.ChildAtIndex);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetDescendants(bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Descendants);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromTargetSiblings()
        {
            binding.SetSearchParameters(SearchOrigin.InjectionTarget, SearchDirection.Siblings);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region CUSTOM TARGET LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> From(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Self);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromParentOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Parent);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromAncestorsOf(
            Transform target,
            bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Ancestors);
            binding.SetCustomTargetTransform(target);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromFirstChildOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.FirstChild);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromLastChildOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.LastChild);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.ChildAtIndex);
            binding.SetCustomTargetTransform(target);
            binding.SetChildIndexForSearch(index);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromDescendantsOf(
            Transform target,
            bool includeSelf = false)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Descendants);
            binding.SetCustomTargetTransform(target);
            binding.SetIncludeSelfInSearch(includeSelf);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromSiblingsOf(Transform target)
        {
            binding.SetSearchParameters(SearchOrigin.CustomTargetTransform, SearchDirection.Siblings);
            binding.SetCustomTargetTransform(target);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region SPECIAL LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> anywhere in the scene using <see cref="UnityEngine.Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromAnywhereInScene(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            binding.SetSearchParameters(SearchOrigin.Scene, SearchDirection.Anywhere);
            binding.SetSceneSearchFindObjectsSettings(findObjectsInactive, sortMode);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromInstance(TComponent instance)
        {
            binding.SetSearchParameters(SearchOrigin.SingleInstance, SearchDirection.None);
            binding.ResolveFromInstances(instance);
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromMethod(Func<TComponent> method)
        {
            binding.SetSearchParameters(SearchOrigin.SingleInstance, SearchDirection.None);
            binding.ResolveFromInstances(method?.Invoke());
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Component" />s using the provided method.
        /// </summary>
        public NewComponentFilterBuilder<TComponent> FromMethod(Func<IEnumerable<TComponent>> method)
        {
            binding.SetSearchParameters(SearchOrigin.MultipleInstances, SearchDirection.None);
            binding.ResolveFromInstances(method?.Invoke().Cast<Component>());
            binding.MarkLocatorStrategySpecified();
            return new NewComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Creates or locates a <see cref="Plugins.Saneject.Runtime.Proxy.ProxyObject{TConcrete}" /> for <c>TComponent</c>, acting as a weak reference that resolves to a concrete <see cref="Component" /> at runtime. This enables serializing references across boundaries Unity normally can’t (e.g. between scenes or prefabs). Uses the first existing proxy project-wide or generates a new one, including a stub script and proxy ScriptableObject asset if missing.
        /// </summary>
        public void FromProxy()
        {
            binding.MarkResolveFromProxy();
            binding.MarkLocatorStrategySpecified();
        }

        #endregion
    }
}