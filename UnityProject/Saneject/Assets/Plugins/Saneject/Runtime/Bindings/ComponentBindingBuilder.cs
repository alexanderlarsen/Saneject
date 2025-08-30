using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for configuring component bindings within a <see cref="Scope" />.
    /// Allows specifying how to locate a <see cref="Component" /> instance (or collection) from the scene hierarchy or injection context.
    /// Supports locating from the scope, injection target, custom transforms, scene-wide queries, or explicit instances.
    /// Typically returned from binding methods like <c>BindComponent&lt;TComponent&gt;()</c> or <c>BindMultipleComponents&lt;TComponent&gt;()</c>.
    /// </summary>
    public class ComponentBindingBuilder<TComponent> where TComponent : class
    {
        private readonly Binding binding;
        private readonly Scope scope;

        public ComponentBindingBuilder(
            Binding binding,
            Scope scope)
        {
            this.binding = binding;
            this.scope = scope;
        }

        #region QUALIFIER METHODS

        /// <summary>
        /// Qualifies this binding with an ID.
        /// Only injection targets annotated with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />
        /// that specify the same ID will resolve using this binding.
        /// </summary>
        /// <param name="ids">The identifiers to match against injection targets.</param>
        public ComponentBindingBuilder<TComponent> ToID(params string[] ids)
        {
            foreach (string id in ids)
                binding.AddIdQualifier(fieldId => fieldId == id, id);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is of the given type.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <typeparam name="TTarget">The target type this binding applies to.</typeparam>
        public ComponentBindingBuilder<TComponent> ToTarget<TTarget>()
        {
            binding.AddInjectionTargetQualifier(obj => obj is TTarget, typeof(TTarget));
            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target is one of the specified types.
        /// The injection target is the <see cref="Component" /> that owns the field or property
        /// marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        /// <param name="targetTypes">One or more target <see cref="Type" /> objects to match against.</param>
        public ComponentBindingBuilder<TComponent> ToTarget(params Type[] targetTypes)
        {
            foreach (Type t in targetTypes)
                binding.AddInjectionTargetQualifier(obj => t.IsInstanceOfType(obj), t);

            return this;
        }

        /// <summary>
        /// Qualifies this binding to apply only when the injection target member (field or property) has one of the specified names.
        /// </summary>
        /// <param name="memberNames">The field or property names on the injection target that this binding should apply to.</param>
        public ComponentBindingBuilder<TComponent> ToMember(params string[] memberNames)
        {
            foreach (string memberName in memberNames)
                binding.AddInjectionTargetMemberQualifier(name => name == memberName, memberName);

            return this;
        }

        #endregion

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
            binding.SetLocator(_ => scope.transform
                .GetComponents<TComponent>()
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeParent()
        {
            binding.SetLocator(_ =>
                scope.transform.parent
                    ? scope.transform.parent
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeAncestors(bool includeSelf = false)
        {
            binding.SetLocator(_ => scope.transform
                .GetComponentsInParents<TComponent>(includeSelf)
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeFirstChild()
        {
            binding.SetLocator(_ =>
                scope.transform.childCount > 0
                    ? scope.transform
                        .GetChild(0)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeLastChild()
        {
            binding.SetLocator(_ =>
                scope.transform.childCount > 0
                    ? scope.transform
                        .GetChild(scope.transform.childCount - 1)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeChildWithIndex(int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < scope.transform.childCount
                    ? scope.transform
                        .GetChild(index)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeDescendants(bool includeSelf = false)
        {
            binding.SetLocator(_ =>
                includeSelf
                    ? scope.transform
                        .GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                    : scope.transform.GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                        .Where(c => c.gameObject != scope.gameObject));

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromScopeSiblings()
        {
            binding.SetLocator(_ =>
                scope.transform.parent
                    ? scope.transform.parent.Cast<Transform>()
                        .Where(t => t != scope.transform)
                        .SelectMany(t => t.GetComponents<TComponent>())
                        .Cast<Component>()
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region ROOT LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootSelf()
        {
            binding.SetLocator(_ => scope.transform.root
                .GetComponents<TComponent>()
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootFirstChild()
        {
            binding.SetLocator(_ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root
                        .GetChild(0)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootLastChild()
        {
            binding.SetLocator(_ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root
                        .GetChild(scope.transform.root.childCount - 1)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootChildWithIndex(int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < scope.transform.root.childCount
                    ? scope.transform.root
                        .GetChild(index)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromRootDescendants(bool includeSelf = false)
        {
            binding.SetLocator(_ =>
                includeSelf
                    ? scope.transform.root
                        .GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                    : scope.transform.root
                        .GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                        .Where(c => c.gameObject != scope.transform.root.gameObject));

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
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetParent()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp && comp.transform.parent
                    ? comp.transform.parent
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetAncestors(bool includeSelf = false)
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform
                        .GetComponentsInParents<TComponent>(includeSelf)
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetFirstChild()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform
                        .GetChild(0)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetLastChild()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform
                        .GetChild(comp.transform.childCount - 1)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetChildWithIndex(int index)
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp && index >= 0 && index < comp.transform.childCount
                    ? comp.transform
                        .GetChild(index)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetDescendants(bool includeSelf = false)
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? includeSelf
                        ? comp
                            .GetComponentsInChildren<TComponent>(true)
                            .Cast<Component>()
                        : comp
                            .GetComponentsInChildren<TComponent>(true)
                            .Cast<Component>()
                            .Where(c => c && c.gameObject != comp.gameObject)
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the Transform of the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromTargetSiblings()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform.parent
                        ? comp.transform.parent.Cast<Transform>()
                            .Where(t => t != comp.transform)
                            .SelectMany(t => t.GetComponents<TComponent>())
                            .Cast<Component>()
                            .Where(c => c)
                        : Enumerable.Empty<Object>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region CUSTOM TARGET LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> From(Transform target)
        {
            binding.SetLocator(_ => target
                .GetComponents<TComponent>()
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromParentOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.parent
                    ? target.parent
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

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
            binding.SetLocator(_ => target
                .GetComponentsInParents<TComponent>(includeSelf)
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromFirstChildOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.childCount > 0
                    ? target
                        .GetChild(0)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromLastChildOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.childCount > 0
                    ? target
                        .GetChild(target.childCount - 1)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < target.childCount
                    ? target
                        .GetChild(index)
                        .GetComponents<TComponent>()
                        .Cast<Component>()
                    : Enumerable.Empty<Object>());

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
            binding.SetLocator(_ =>
                includeSelf
                    ? target
                        .GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                    : target
                        .GetComponentsInChildren<TComponent>(true)
                        .Cast<Component>()
                        .Where(c => c.gameObject != target.gameObject));

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromSiblingsOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.parent
                    ? target.parent.Cast<Transform>()
                        .Where(t => t != target)
                        .SelectMany(t => t.GetComponents<TComponent>())
                        .Cast<Component>()
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        #endregion

        #region SPECIAL LOCATOR METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> anywhere in the scene using <see cref="Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromAnywhereInScene(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            binding.SetLocator(_ => Object
                .FindObjectsByType<Component>(findObjectsInactive, sortMode)
                .OfType<TComponent>()
                .Cast<Component>());

            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromInstance(TComponent instance)
        {
            binding.SetLocator(_ => instance.WrapInEnumerable().Cast<Component>());
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<TComponent> method)
        {
            binding.SetLocator(_ => method?.Invoke().WrapInEnumerable().Cast<Component>());
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Locate multiple <see cref="Component" />s using the provided method.
        /// </summary>
        public ComponentFilterBuilder<TComponent> FromMethod(Func<IEnumerable<TComponent>> method)
        {
            binding.SetLocator(_ => method?.Invoke().Cast<Component>());
            return new ComponentFilterBuilder<TComponent>(binding);
        }

        /// <summary>
        /// Creates or locates a <see cref="Plugins.Saneject.Runtime.Proxy.ProxyObject{TConcrete}" /> for <c>TComponent</c>, acting as a weak reference that resolves to a concrete <see cref="Component" /> at runtime. This enables serializing references across boundaries Unity normally can’t (e.g. between scenes or prefabs). Uses the first existing proxy project-wide or generates a new one, including a stub script and proxy ScriptableObject asset if missing.
        /// </summary>
        public void FromProxy()
        {
            binding.MarkResolveWithProxy();
            binding.SetLocator(_ => null);
        }

        #endregion
    }
}