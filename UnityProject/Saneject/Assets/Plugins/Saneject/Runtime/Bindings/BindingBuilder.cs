using System;
using System.Linq;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for registering a component binding in a <see cref="Plugins.Saneject.Runtime.Scopes.Scope" />.
    /// Used to define how dependencies of type <c>T</c> (a <see cref="Component" />) are located and injected.
    /// Typical usage: call <see cref="Scope.RegisterComponent{T}" /> or <see cref="Scope.RegisterComponent{TInterface,TConcrete}" />
    /// and chain with methods on this builder to specify the search strategy for the dependency.
    /// </summary>
    /// <typeparam name="T">The <see cref="Component" /> type to bind.</typeparam>
    public class BindingBuilder<T> where T : Object
    {
        public readonly Binding binding;

        private readonly Scope scope;

        public BindingBuilder(
            Scope scope,
            Type concreteType)
        {
            this.scope = scope;
            binding = new Binding(null, concreteType);
        }

        public BindingBuilder(
            Scope scope,
            Type interfaceType,
            Type concreteType)
        {
            this.scope = scope;
            binding = new Binding(interfaceType, concreteType);
        }

        // ---------- DEFAULT (SCOPE) METHODS START ----------

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeSelf" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromSelf()
        {
            return FromScopeSelf();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// Shorthand for <see cref="FromScopeParent" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromParent()
        {
            return FromScopeParent();
        }

        /// <summary>
        /// Locate the component on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeAncestors" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromAncestors()
        {
            return FromScopeAncestors();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeFirstChild" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromFirstChild()
        {
            return FromScopeFirstChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// Shorthand for <see cref="FromScopeLastChild" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromLastChild()
        {
            return FromScopeLastChild();
        }

        /// <summary>
        /// Locate the component on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// Shorthand for <see cref="FromScopeChildWithIndex" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromChildWithIndex(int index)
        {
            return FromScopeChildWithIndex(index);
        }

        /// <summary>
        /// Locate the component on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Shorthand for <see cref="FromScopeDescendants" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromDescendants(bool includeSelf = true)
        {
            return FromScopeDescendants(includeSelf);
        }

        /// <summary>
        /// Locate the component on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// Shorthand for <see cref="FromScopeSiblings" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromSiblings()
        {
            return FromScopeSiblings();
        }

        // ---------- DEFAULT (SCOPE) METHODS END ----------

        // ---------- SCOPE METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeSelf()
        {
            binding.SetLocator(_ => scope.transform.GetComponent<T>().WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeParent()
        {
            binding.SetLocator(_ =>
                scope.transform.parent
                    ? scope.transform.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeAncestors()
        {
            binding.SetLocator(_ => scope.transform.GetComponentsInParents<T>());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeFirstChild()
        {
            binding.SetLocator(_ =>
                scope.transform.childCount > 0
                    ? scope.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeLastChild()
        {
            binding.SetLocator(_ =>
                scope.transform.childCount > 0
                    ? scope.transform.GetChild(scope.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeChildWithIndex(int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < scope.transform.childCount
                    ? scope.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeDescendants(bool includeSelf = true)
        {
            binding.SetLocator(_ =>
                includeSelf
                    ? scope.transform.GetComponentsInChildren<T>(true)
                    : scope.transform.GetComponentsInChildren<T>(true).Where(t => t is Component c && c.gameObject != scope.gameObject));

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeSiblings()
        {
            binding.SetLocator(_ =>
                scope.transform.parent
                    ? scope.transform.parent.Cast<Transform>()
                        .Where(t => t != scope.transform)
                        .Select(t => t.GetComponent<T>())
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        // ---------- SCOPE METHODS END ----------

        // ---------- ROOT METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootSelf()
        {
            binding.SetLocator(_ => scope.transform.root.GetComponent<T>().WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootFirstChild()
        {
            binding.SetLocator(_ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootLastChild()
        {
            binding.SetLocator(_ =>
                scope.transform.root.childCount > 0
                    ? scope.transform.root.GetChild(scope.transform.root.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootChildWithIndex(int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < scope.transform.root.childCount
                    ? scope.transform.root.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootDescendants(bool includeSelf = true)
        {
            binding.SetLocator(_ =>
                includeSelf
                    ? scope.transform.root.GetComponentsInChildren<T>(true)
                    : scope.transform.root.GetComponentsInChildren<T>(true).Where(t => t is Component c && c.gameObject != scope.transform.root.gameObject));

            return new FilterableBindingBuilder<T>(binding);
        }

        // ---------- ROOT METHODS END ----------

        // ---------- INJECTION TARGET METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetSelf()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetParent()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp && comp.transform.parent
                    ? comp.transform.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetAncestors()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform.GetComponentsInParents<T>()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetFirstChild()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetLastChild()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component { transform: { childCount: > 0 } } comp
                    ? comp.transform.GetChild(comp.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetChildWithIndex(int index)
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp && index >= 0 && index < comp.transform.childCount
                    ? comp.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetDescendants(bool includeSelf = true)
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? includeSelf
                        ? comp.GetComponentsInChildren<T>(true)
                        : comp.GetComponentsInChildren<T>(true).Where(t => t is Component c && c.gameObject != comp.gameObject)
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetSiblings()
        {
            binding.MarkRequireInjectionTarget();

            binding.SetLocator(injectionTarget =>
                injectionTarget is Component comp
                    ? comp.transform.parent
                        ? comp.transform.parent.Cast<Transform>()
                            .Where(t => t != comp.transform)
                            .Select(t => t.GetComponent<T>())
                            .Where(c => c)
                        : Enumerable.Empty<Object>()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        // ---------- INJECTION TARGET METHODS END ----------

        // ---------- CUSTOM TARGET METHODS START ----------

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> From(Transform target)
        {
            binding.SetLocator(_ => target.GetComponent<T>().WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromParentOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.parent
                    ? target.parent.GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromAncestorsOf(Transform target)
        {
            binding.SetLocator(_ => target.GetComponentsInParents<T>());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromFirstChildOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.childCount > 0
                    ? target.GetChild(0).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromLastChildOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.childCount > 0
                    ? target.GetChild(target.childCount - 1).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            binding.SetLocator(_ =>
                index >= 0 && index < target.childCount
                    ? target.GetChild(index).GetComponent<T>().WrapInEnumerable()
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromDescendantsOf(
            Transform target,
            bool includeSelf = true)
        {
            binding.SetLocator(_ =>
                includeSelf
                    ? target.GetComponentsInChildren<T>(true)
                    : target.GetComponentsInChildren<T>(true).Where(t => t is Component c && c.gameObject != target.gameObject));

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public FilterableBindingBuilder<T> FromSiblingsOf(Transform target)
        {
            binding.SetLocator(_ =>
                target.parent
                    ? target.parent.Cast<Transform>()
                        .Where(t => t != target)
                        .Select(t => t.GetComponent<T>())
                        .Where(c => c)
                    : Enumerable.Empty<Object>());

            return new FilterableBindingBuilder<T>(binding);
        }
// ---------- CUSTOM TARGET METHODS END ----------

// ---------- SPECIAL METHODS START ----------

/// <summary>
/// Locate the <see cref="Component" /> anywhere in the scene using <see cref="Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
/// </summary>
public FilterableBindingBuilder<T> FromAnywhereInScene()
        {
            binding.SetLocator(_ => Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Bind to the specified <see cref="Component" /> instance.
        /// </summary>
        public void FromInstance(T instance)
        {
            binding.SetLocator(_ => instance.WrapInEnumerable());
        }

        /// <summary>
        /// Locate the <see cref="Component" /> using the provided method.
        /// </summary>
        public FilterableBindingBuilder<T> FromMethod(Func<T> method)
        {
            binding.SetLocator(_ => method?.Invoke().WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.Load(string)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromResources(string path)
        {
            binding.SetLocator(_ => Resources.Load<T>(path).WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromResourcesAll(string path)
        {
            binding.SetLocator(_ => Resources.LoadAll<T>(path));
            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromAssetLoad(string assetPath)
        {
#if UNITY_EDITOR
            binding.SetLocator(_ => AssetDatabase.LoadAssetAtPath<T>(assetPath).WrapInEnumerable());
            return new FilterableBindingBuilder<T>(binding);
#else
            return null;
#endif
        }

        /// <summary>
        /// Set the binding ID for resolution of fields/properties with the same ID in their <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public BindingBuilder<T> WithId(string id)
        {
            binding.SetId(id);
            return this;
        }

        public BindingBuilder<T> AsGlobal()
        {
            binding.MarkGlobal();
            return this;
        }
// ---------- SPECIAL METHODS END ----------
    }
}