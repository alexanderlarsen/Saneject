using System;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for applying filters to component bindings within a <see cref="Scope" />.
    /// Used to refine selection of <see cref="UnityEngine.Component" /> instances after a locator has been specified.
    /// Supports filtering by GameObject state, transform hierarchy, component order, activation status, custom predicates, and injection target type.
    /// Returned from locator methods on <c>ComponentBindingBuilder&lt;TComponent&gt;</c>.
    /// </summary>
    public class ComponentFilterBuilder<TComponent> : BaseFilterBuilder where TComponent : class
    {
        private readonly Binding binding;
        private readonly Scope scope;

        public ComponentFilterBuilder(
            Binding binding,
            Scope scope)
        {
            this.binding = binding;
            this.scope = scope;
        }

        #region BEHAVIOUR METHODS

        /// <summary>
        /// Filter to only search for <see cref="Behaviour" /> components where <see cref="Behaviour.enabled" /> is true.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsEnabled()
        {
            if (typeof(Behaviour).IsAssignableFrom(typeof(TComponent)))
                binding.AddFilter(o => o is Behaviour { enabled: true });
            else
                Debug.LogError($"Saneject: 'ComponentFilterBuilder<T>.{nameof(WhereComponentIndexIs)}()' can only be used with Behaviour types. '{typeof(TComponent)}' is not a Behaviour.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Behaviour" /> components where <see cref="Behaviour.isActiveAndEnabled" /> is true.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsActiveAndEnabled()
        {
            if (typeof(Behaviour).IsAssignableFrom(typeof(TComponent)))
                binding.AddFilter(o => o is Behaviour { isActiveAndEnabled: true });
            else
                Debug.LogError($"Saneject: 'ComponentFilterBuilder<T>.{nameof(WhereComponentIndexIs)}()' can only be used with Behaviour types. '{typeof(TComponent)}' is not a Behaviour.", scope);

            return this;
        }

        #endregion

        #region COMPONENT METHODS

        /// <summary>
        /// Filter to only search for <see cref="Component" />s with the given component index on their <see cref="GameObject" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereComponentIndexIs(int index)
        {
            binding.AddFilter(o => o is Component component && component.GetComponentIndex() == index);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s that are the first component on their <see cref="GameObject" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsFirstComponentSibling()
        {
            binding.AddFilter(o => o is Component component && component.GetComponentIndex() == 0);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s that are the last component on their <see cref="GameObject" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsLastComponentSibling()
        {
            binding.AddFilter(o => o is Component component && component.GetComponentIndex() == component.transform.childCount - 1);
            return this;
        }

        #endregion

        #region GAMEOBJECT METHODS

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s have a specific <see cref="GameObject.name" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereNameIs(string name)
        {
            binding.AddFilter(o => GetObjectName(o) == name);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s have <see cref="GameObject.name" />s that contain a substring.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereNameContains(string substring)
        {
            binding.AddFilter(o => GetObjectName(o)?.Contains(substring) == true);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s have a specific <see cref="GameObject.tag" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereTagIs(string tag)
        {
            binding.AddFilter(o => GetGameObject(o)?.CompareTag(tag) == true);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s have a specific <see cref="GameObject.layer" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereLayerIs(int layer)
        {
            binding.AddFilter(o => GetGameObject(o)?.layer == layer);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s are active in the hierarchy.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereActiveInHierarchy()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == true);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s are inactive in the hierarchy.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereInactiveInHierarchy()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == false);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s local state is active.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereActiveSelf()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeSelf == true);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="GameObject" />s local state is inactive.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereInactiveSelf()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeSelf == false);
            return this;
        }

        #endregion

        #region TRANSFORM METHODS

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="Transform" />s have a specific sibling index.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereSiblingIndexIs(int index)
        {
            binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == index);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="Transform" />s are a first sibling.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsFirstSibling()
        {
            binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == 0);
            return this;
        }

        /// <summary>
        /// Filter to only search for <see cref="Component" />s whose <see cref="Transform" />s are a last sibling.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereIsLastSibling()
        {
            binding.AddFilter(o =>
            {
                Transform t = GetTransform(o);
                return t && t.parent && t.GetSiblingIndex() == t.parent.childCount - 1;
            });

            return this;
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Filter using a predicate on <typeparamref name="TComponent" /> for custom search logic.
        /// </summary>
        public ComponentFilterBuilder<TComponent> Where(Func<TComponent, bool> predicate)
        {
            binding.AddFilter(o => o is TComponent t && predicate(t));
            return this;
        }

        /// <summary>
        /// Filter to only resolve dependencies using this binding when the injection target is of type <typeparamref name="TTarget" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public ComponentFilterBuilder<TComponent> WhereTargetIs<TTarget>()
        {
            binding.AddTargetFilter(obj => obj is TTarget, typeof(TTarget));
            return this;
        }

        #endregion
    }
}