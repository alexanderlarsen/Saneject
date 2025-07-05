using System;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for registering a component binding in a <see cref="Plugins.Saneject.Runtime.Scopes.Scope" />.
    /// Used to define how dependencies of type <c>T</c> (a <see cref="Component"/>) are located and injected.
    /// Typical usage: call <see cref="Scope.RegisterComponent{T}"/> or <see cref="Scope.RegisterComponent{TInterface,TConcrete}"/>
    /// and chain with methods on this builder to specify the search strategy for the dependency.
    /// </summary>
    /// <typeparam name="T">The <see cref="Component"/> type to bind.</typeparam>
    public class ComponentBindingBuilder<T> where T : Component
    {
        private readonly Scope scope;
        private readonly Type interfaceType;
        private readonly Type concreteType;
        private string id;

        public ComponentBindingBuilder(
            Scope scope,
            Type concreteType)
        {
            this.scope = scope;
            interfaceType = null;
            this.concreteType = concreteType;
        }

        public ComponentBindingBuilder(
            Scope scope,
            Type interfaceType,
            Type concreteType)
        {
            this.scope = scope;
            this.concreteType = concreteType;
            this.interfaceType = interfaceType;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event Action<Binding> OnBindingCreated;

        // ---------- DEFAULT (SCOPE) METHODS START ----------

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeSelf" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromSelf()
        {
            return FromScopeSelf();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// Shorthand for <see cref="FromScopeParent" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromParent()
        {
            return FromScopeParent();
        }

        /// <summary>
        /// Locate the component on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeAncestors" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromAncestors()
        {
            return FromScopeAncestors();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeFirstChild" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromFirstChild()
        {
            return FromScopeFirstChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeLastChild" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromLastChild()
        {
            return FromScopeLastChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// Shorthand for <see cref="FromScopeChildWithIndex" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromChildWithIndex(int index)
        {
            return FromScopeChildWithIndex(index);
        }

        /// <summary>
        /// Locate the component on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeDescendants" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromDescendants(bool includeSelf = true)
        {
            return FromScopeDescendants(includeSelf);
        }

        /// <summary>
        /// Locate the component on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// Shorthand for <see cref="FromScopeSiblings" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromSiblings()
        {
            return FromScopeSiblings();
        }

        // ---------- DEFAULT (SCOPE) METHODS END ----------

        // ---------- SCOPE METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeSelf()
        {
            Binding binding = new(interfaceType, concreteType, id, _ => scope.transform.GetComponent<T>().WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeParent()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.parent
                    ? scope.transform.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeAncestors()
        {
            Binding binding = new(interfaceType, concreteType, id, _ => scope.transform.GetComponentsInParents<T>());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeFirstChild()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.childCount > 0
                    ? scope.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeLastChild()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.childCount > 0
                    ? scope.transform.GetChild(scope.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeChildWithIndex(int index)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                index >= 0 && index < scope.transform.childCount
                    ? scope.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeDescendants(bool includeSelf = true)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                includeSelf
                    ? scope.transform.GetComponentsInChildren<T>(true)
                    : scope.transform.GetComponentsInChildren<T>(true).Where(c => c.gameObject != scope.gameObject));

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromScopeSiblings()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.parent
                    ? scope.transform.parent.Cast<Transform>()
                        .Where(t => t != scope.transform)
                        .Select(t => t.GetComponent<T>())
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        // ---------- SCOPE METHODS END ----------

        // ---------- ROOT METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromRootSelf()
        {
            Binding binding = new(interfaceType, concreteType, id, _ => scope.transform.root.GetComponent<T>().WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromRootFirstChild()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromRootLastChild()
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root.GetChild(scope.transform.root.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromRootChildWithIndex(int index)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                index >= 0 && index < scope.transform.root.childCount
                    ? scope.transform.root.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromRootDescendants(bool includeSelf = true)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                includeSelf
                    ? scope.transform.root.GetComponentsInChildren<T>(true)
                    : scope.transform.root.GetComponentsInChildren<T>(true).Where(c => c.gameObject != scope.transform.root.gameObject));

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        // ---------- ROOT METHODS END ----------

        // ---------- INJECTION TARGET METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetSelf()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp
                    ? comp.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetParent()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp && comp.transform.parent
                    ? comp.transform.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetAncestors()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform.GetComponentsInParents<T>()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetFirstChild()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetLastChild()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform.GetChild(comp.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetChildWithIndex(int index)
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp && index >= 0 && index < comp.transform.childCount
                    ? comp.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetDescendants(bool includeSelf = true)
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp
                    ? includeSelf
                        ? comp.GetComponentsInChildren<T>(true)
                        : comp.GetComponentsInChildren<T>(true).Where(c => c.gameObject != comp.gameObject)
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromTargetSiblings()
        {
            Binding binding = new(interfaceType, concreteType, id, injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform.parent
                        ? comp.transform.parent.Cast<Transform>()
                            .Where(t => t != comp.transform)
                            .Select(t => t.GetComponent<T>())
                            .Where(c => c)
                        : Enumerable.Empty<Object>()
                    : Enumerable.Empty<Object>(), true);

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        // ---------- INJECTION TARGET METHODS END ----------

        // ---------- CUSTOM TARGET METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> From(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => target.GetComponent<T>().WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromParentOf(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                target.parent
                    ? target.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromAncestorsOf(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => target.GetComponentsInParents<T>());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromFirstChildOf(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                target.childCount > 0
                    ? target.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromLastChildOf(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                target.childCount > 0
                    ? target.GetChild(target.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                index >= 0 && index < target.childCount
                    ? target.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromDescendantsOf(
            Transform target,
            bool includeSelf = true)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                includeSelf
                    ? target.GetComponentsInChildren<T>(true)
                    : target.GetComponentsInChildren<T>(true).Where(c => c.gameObject != target.gameObject));

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromSiblingsOf(Transform target)
        {
            Binding binding = new(interfaceType, concreteType, id, _ =>
                target.parent
                    ? target.parent.Cast<Transform>()
                        .Where(t => t != target)
                        .Select(t => t.GetComponent<T>())
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }
// ---------- CUSTOM TARGET METHODS END ----------

// ---------- SPECIAL METHODS START ----------

/// <summary>
/// Locate the <see cref="Component" /> anywhere in the scene using <see cref="Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
/// </summary>
public FilterableComponentBindingBuilder<T> FromAnywhereInScene()
        {
            Binding binding = new(interfaceType, concreteType, id, _ => Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public void FromInstance(T instance)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => instance.WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public FilterableComponentBindingBuilder<T> FromMethod(Func<T> method)
        {
            Binding binding = new(interfaceType, concreteType, id, _ => method?.Invoke().WrapInEnumerable());
            OnBindingCreated?.Invoke(binding);
            return new FilterableComponentBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Set the binding ID for resolution of fields/properties with the same ID in their <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentBindingBuilder<T> WithId(string id)
        {
            this.id = id;
            return this;
        }
// ---------- SPECIAL METHODS END ----------
    }
}