using System;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for adding filters to a component binding created by <see cref="BindingBuilder{T}" />.
    /// Enables fine-grained selection of <typeparamref name="T" /> (a <see cref="Component" />) by <see cref="GameObject" /> name, tag, layer, active state, sibling index, or custom predicates.
    /// </summary>
    /// <typeparam name="T">The <see cref="Component" /> type to filter.</typeparam>
    public class FilterableBindingBuilder<T> where T : Object
    {
        private readonly Binding binding;
        private readonly Scope scope;

        public FilterableBindingBuilder(
            Binding binding,
            Scope scope)
        {
            this.binding = binding;
            this.scope = scope;
        }

        #region BEHAVIOUR METHODS

        /// <summary>
        /// Filter to only include <see cref="Behaviour" />s where <see cref="Behaviour.enabled" /> is true.
        /// </summary>
        public FilterableBindingBuilder<T> WhereBehaviourIsEnabled()
        {
            if (typeof(Behaviour).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => o is Behaviour { enabled: true });
            else
                Debug.LogError($"Saneject: '{nameof(WhereComponentIndexIs)}' can only be used with Behaviour types. '{typeof(T)}' is not a Behaviour.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Behaviour" />s where <see cref="Behaviour.isActiveAndEnabled" /> is true.
        /// </summary>
        public FilterableBindingBuilder<T> WhereBehaviourIsActiveAndEnabled()
        {
            if (typeof(Behaviour).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => o is Behaviour { isActiveAndEnabled: true });
            else
                Debug.LogError($"Saneject: '{nameof(WhereComponentIndexIs)}' can only be used with Behaviour types. '{typeof(T)}' is not a Behaviour.", scope);

            return this;
        }

        #endregion

        #region COMPONENT METHODS

        /// <summary>
        /// Filter to only include <see cref="Component" />s with the given index on their <see cref="GameObject" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereComponentIndexIs(int index)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => o is Component component && component.GetComponentIndex() == index);
            else
                Debug.LogError($"Saneject: '{nameof(WhereComponentIndexIs)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s that are the first on their <see cref="GameObject" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereComponentIsFirstSibling()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => o is Component component && component.GetComponentIndex() == 0);
            else
                Debug.LogError($"Saneject: '{nameof(WhereComponentIndexIs)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s that are the last on their <see cref="GameObject" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereComponentIsLastSibling()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => o is Component component && component.GetComponentIndex() == component.transform.childCount - 1);
            else
                Debug.LogError($"Saneject: '{nameof(WhereComponentIndexIs)}' can only be used with Component types. '{typeof(T)}' is not a Component.", scope);

            return this;
        }

        #endregion

        #region GAMEOBJECT METHODS

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.tag" /> matches <paramref name="tag" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereGameObjectTagIs(string tag)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetGameObject(o)?.CompareTag(tag) == true);
            else
                Debug.LogError($"Saneject: '{nameof(WhereGameObjectTagIs)}' can only be used with Component or GameObject types. '{typeof(T)}' is not a Component or GameObject.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> contains <paramref name="substring" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereGameObjectNameContains(string substring)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetObjectName(o)?.Contains(substring) == true);
            else
                Debug.LogError($"Saneject: '{nameof(WhereGameObjectNameContains)}' can only be used with Component or GameObject types. '{typeof(T)}' is not a Component or GameObject.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.layer" /> matches <paramref name="layer" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereGameObjectLayerIs(int layer)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetGameObject(o)?.layer == layer);
            else
                Debug.LogError($"Saneject: '{nameof(WhereGameObjectLayerIs)}' can only be used with Component or GameObject types. '{typeof(T)}' is not a Component or GameObject.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is active in the hierarchy.
        /// </summary>
        public FilterableBindingBuilder<T> WhereGameObjectActiveInHierarchy()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == true);
            else
                Debug.LogError($"Saneject: '{nameof(WhereGameObjectActiveInHierarchy)}' can only be used with Component or GameObject types. '{typeof(T)}' is not a Component or GameObject.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is inactive in the hierarchy.
        /// </summary>
        public FilterableBindingBuilder<T> WhereGameObjectInactiveInHierarchy()
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == false);
            else
                Debug.LogError($"Saneject: '{nameof(WhereGameObjectInactiveInHierarchy)}' can only be used with Component or GameObject types. '{typeof(T)}' is not a Component or GameObject.", scope);

            return this;
        }

        #endregion

        #region TRANSFORM METHODS

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> has the given sibling index.
        /// </summary>
        public FilterableBindingBuilder<T> WhereTransformSiblingIndexIs(int index)
        {
            if (typeof(Transform).IsAssignableFrom(typeof(T)) || typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == index);
            else
                Debug.LogError($"Saneject: '{nameof(WhereTransformSiblingIndexIs)}' can only be used with Component, GameObject, or Transform types. '{typeof(T)}' is not a Component, GameObject, or Transform.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the first sibling.
        /// </summary>
        public FilterableBindingBuilder<T> WhereTransformIsFirstSibling()
        {
            if (typeof(Transform).IsAssignableFrom(typeof(T)) || typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == 0);
            else
                Debug.LogError($"Saneject: '{nameof(WhereTransformSiblingIndexIs)}' can only be used with Component, GameObject, or Transform types. '{typeof(T)}' is not a Component, GameObject, or Transform.", scope);

            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the last sibling.
        /// </summary>
        public FilterableBindingBuilder<T> WhereTransformIsLastSibling()
        {
            if (typeof(Transform).IsAssignableFrom(typeof(T)) || typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
                binding.AddFilter(o =>
                {
                    Transform t = GetTransform(o);
                    return t && t.parent && t.GetSiblingIndex() == t.parent.childCount - 1;
                });
            else
                Debug.LogError($"Saneject: '{nameof(WhereTransformSiblingIndexIs)}' can only be used with Component, GameObject, or Transform types. '{typeof(T)}' is not a Component, GameObject, or Transform.", scope);

            return this;
        }

        #endregion

        #region SPECIAL METHODS

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> matches <paramref name="name" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereNameIs(string name)
        {
            binding.AddFilter(o => GetObjectName(o) == name);
            return this;
        }

        /// <summary>
        /// Filter using a custom predicate on <typeparamref name="T" />.
        /// </summary>
        public FilterableBindingBuilder<T> Where(Func<T, bool> predicate)
        {
            binding.AddFilter(o => o is T t && predicate(t));
            return this;
        }

        /// <summary>
        /// Filter to only resolve dependencies when the injection target is of type <typeparamref name="TTarget" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereTargetIs<TTarget>()
        {
            binding.AddTargetFilter(target => target is TTarget);
            return this;
        }

        #endregion

        #region STATIC HELPER METHODS

        private static Transform GetTransform(Object o)
        {
            return o switch
            {
                Transform t => t,
                Component c => c.transform,
                GameObject go => go.transform,
                _ => null
            };
        }

        private static string GetObjectName(Object o)
        {
            return o switch
            {
                Component c => c.gameObject.name,
                GameObject go => go.name,
                _ => o?.name
            };
        }

        private static GameObject GetGameObject(Object o)
        {
            return o switch
            {
                GameObject go => go,
                Component c => c.gameObject,
                _ => null
            };
        }

        #endregion
    }
}