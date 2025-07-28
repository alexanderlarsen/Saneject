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
    /// Typical usage: call <see cref="Scope.Bind{T}" /> or <see cref="Scope.Bind{TInterface,TConcrete}" />
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
            binding = new Binding(null, concreteType, scope);
        }

        public BindingBuilder(
            Scope scope,
            Type interfaceType,
            Type concreteType)
        {
            this.scope = scope;
            binding = new Binding(interfaceType, concreteType, scope);
        }

        #region DEFAULT (SCOPE) METHODS

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

        #endregion

        #region SCOPE METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s own <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeSelf()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => scope.transform.GetComponent<T>().WrapInEnumerable());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeSelf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct <see cref="Transform.parent" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeParent()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.parent
                        ? scope.transform.parent.GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeParent)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeAncestors()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => scope.transform.GetComponentsInParents<T>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeAncestors)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeFirstChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.childCount > 0
                        ? scope.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeFirstChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeLastChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.childCount > 0
                        ? scope.transform.GetChild(scope.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeLastChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeChildWithIndex(int index)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    index >= 0 && index < scope.transform.childCount
                        ? scope.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeChildWithIndex)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeDescendants(bool includeSelf = true)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    includeSelf
                        ? scope.transform.GetComponentsInChildren<T>(true)
                        : scope.transform.GetComponentsInChildren<T>(true)
                            .Where(t => t is Component c && c.gameObject != scope.gameObject));
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeDescendants)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the <see cref="Scope" /> (other children of the same parent).
        /// </summary>
        public FilterableBindingBuilder<T> FromScopeSiblings()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.parent
                        ? scope.transform.parent.Cast<Transform>()
                            .Where(t => t != scope.transform)
                            .Select(t => t.GetComponent<T>())
                            .Where(c => c)
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromScopeSiblings)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        #endregion

        #region ROOT METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootSelf()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => scope.transform.root.GetComponent<T>().WrapInEnumerable());
            else
                Debug.LogError($"Saneject: '{nameof(FromRootSelf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootFirstChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.root.childCount > 0
                        ? scope.transform.root.GetChild(0).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromRootFirstChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootLastChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    scope.transform.root.childCount > 0
                        ? scope.transform.root.GetChild(scope.transform.root.childCount - 1).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromRootLastChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootChildWithIndex(int index)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    index >= 0 && index < scope.transform.root.childCount
                        ? scope.transform.root.GetChild(index).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromRootChildWithIndex)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the <see cref="Scope" /> <see cref="Transform" /> <see cref="Transform.root" />.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromRootDescendants(bool includeSelf = true)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    includeSelf
                        ? scope.transform.root.GetComponentsInChildren<T>(true)
                        : scope.transform.root.GetComponentsInChildren<T>(true)
                            .Where(t => t is Component c && c.gameObject != scope.transform.root.gameObject));
            else
                Debug.LogError($"Saneject: '{nameof(FromRootDescendants)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        #endregion

        #region INJECTION TARGET METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's own <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetSelf()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component comp
                        ? comp.GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetSelf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct <see cref="Transform.parent" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetParent()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component comp && comp.transform.parent
                        ? comp.transform.parent.GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetParent)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the injection target.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetAncestors()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component comp
                        ? comp.transform.GetComponentsInParents<T>()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetAncestors)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's first direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetFirstChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component { transform: { childCount: > 0 } } comp
                        ? comp.transform.GetChild(0).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetFirstChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's last direct child <see cref="Transform" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetLastChild()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component { transform: { childCount: > 0 } } comp
                        ? comp.transform.GetChild(comp.transform.childCount - 1).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetLastChild)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the injection target's direct child <see cref="Transform" /> with index.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetChildWithIndex(int index)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component comp && index >= 0 && index < comp.transform.childCount
                        ? comp.transform.GetChild(index).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetChildWithIndex)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any descendant <see cref="Transform" /> of the injection target.
        /// Searches recursively downwards in all children, grandchildren, etc. until it finds a match.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetDescendants(bool includeSelf = true)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                binding.MarkRequireInjectionTarget();

                binding.SetLocator(injectionTarget =>
                    injectionTarget is Component comp
                        ? includeSelf
                            ? comp.GetComponentsInChildren<T>(true)
                            : comp.GetComponentsInChildren<T>(true).Where(t => t is Component c && c.gameObject != comp.gameObject)
                        : Enumerable.Empty<Object>());
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetDescendants)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the injection target (other children of the same parent).
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromTargetSiblings()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
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
            }
            else
            {
                Debug.LogError($"Saneject: '{nameof(FromTargetSiblings)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);
            }

            return new FilterableBindingBuilder<T>(binding);
        }

        #endregion

        #region CUSTOM TARGET METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> From(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => target.GetComponent<T>().WrapInEnumerable());
            else
                Debug.LogError($"Saneject: '{nameof(From)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the direct parent of the specified <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromParentOf(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    target.parent
                        ? target.parent.GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromParentOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any ancestor <see cref="Transform" /> of the specified <see cref="Transform" />.
        /// Searches recursively upwards in all parents, grandparents, etc. until it finds a match.
        /// </summary>
        public FilterableBindingBuilder<T> FromAncestorsOf(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => target.GetComponentsInParents<T>());
            else
                Debug.LogError($"Saneject: '{nameof(FromAncestorsOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s first direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromFirstChildOf(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    target.childCount > 0
                        ? target.GetChild(0).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromFirstChildOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s last direct child <see cref="Transform" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromLastChildOf(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    target.childCount > 0
                        ? target.GetChild(target.childCount - 1).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromLastChildOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on the specified <see cref="Transform" />'s direct child <see cref="Transform" /> with index.
        /// </summary>
        public FilterableBindingBuilder<T> FromChildWithIndexOf(
            Transform target,
            int index)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    index >= 0 && index < target.childCount
                        ? target.GetChild(index).GetComponent<T>().WrapInEnumerable()
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromChildWithIndexOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

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
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    includeSelf
                        ? target.GetComponentsInChildren<T>(true)
                        : target.GetComponentsInChildren<T>(true)
                            .Where(t => t is Component c && c.gameObject != target.gameObject));
            else
                Debug.LogError($"Saneject: '{nameof(FromDescendantsOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Component" /> on any sibling <see cref="Transform" /> of the specified <see cref="Transform" /> (other children of the same parent).
        /// </summary>
        public FilterableBindingBuilder<T> FromSiblingsOf(Transform target)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ =>
                    target.parent
                        ? target.parent.Cast<Transform>()
                            .Where(t => t != target)
                            .Select(t => t.GetComponent<T>())
                            .Where(c => c)
                        : Enumerable.Empty<Object>());
            else
                Debug.LogError($"Saneject: '{nameof(FromSiblingsOf)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return new FilterableBindingBuilder<T>(binding);
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Locate the <see cref="Component" /> anywhere in the scene using <see cref="Object.FindObjectsByType(Type, FindObjectsSortMode)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromAnywhereInScene(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include,
            FindObjectsSortMode sortMode = FindObjectsSortMode.None)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.SetLocator(_ => Object.FindObjectsByType<T>(findObjectsInactive, sortMode));
            else
                Debug.LogError($"Saneject: '{nameof(FromAnywhereInScene)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

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
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                Debug.LogError($"Saneject: '{nameof(FromResources)}' cannot be used with Component types. '{typeof(T)}' is a Component.", scope);
            else
                binding.SetLocator(_ => Resources.Load<T>(path).WrapInEnumerable());

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate all <see cref="Object" />s in a <see cref="Resources" /> folder at the specified path using <see cref="Resources.LoadAll(string, System.Type)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromResourcesAll(string path)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                Debug.LogError($"Saneject: '{nameof(FromResourcesAll)}' cannot be used with Component types. '{typeof(T)}' is a Component.", scope);
            else
                binding.SetLocator(_ => Resources.LoadAll<T>(path));

            return new FilterableBindingBuilder<T>(binding);
        }

        /// <summary>
        /// Locate the <see cref="Object" /> asset at the specified path using <see cref="UnityEditor.AssetDatabase.LoadAssetAtPath(string, System.Type)" />.
        /// </summary>
        public FilterableBindingBuilder<T> FromAssetLoad(string assetPath)
        {
#if UNITY_EDITOR
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                Debug.LogError($"Saneject: '{nameof(FromAssetLoad)}' cannot be used with Component types. '{typeof(T)}' is a Component.", scope);
            else
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

        /// <summary>
        /// Marks this binding as a global singleton, making it eligible for use in <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> resolution by adding it to a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" />.
        /// </summary>
        public BindingBuilder<T> AsGlobalSingleton()
        {
            binding.MarkGlobal();
            return this;
        }

        /// <summary>
        /// Marks this binding as eligible for collection injection, allowing it to be resolved into array or list fields.
        /// </summary>
        public BindingBuilder<T> AsCollection()
        {
            binding.MarkCollectionBinding();
            return this;
        }

        #endregion
    }
}