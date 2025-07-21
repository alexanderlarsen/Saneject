using System;
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

        public FilterableBindingBuilder(Binding binding)
        {
            this.binding = binding;
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

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.tag" /> matches <paramref name="tag" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereTagIs(string tag)
        {
            binding.AddFilter(o => GetGameObject(o)?.CompareTag(tag) == true);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> matches <paramref name="name" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereNameIs(string name)
        {
            binding.AddFilter(o => GetObjectName(o) == name);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> contains <paramref name="substring" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereNameContains(string substring)
        {
            binding.AddFilter(o => GetObjectName(o)?.Contains(substring) == true);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.layer" /> matches <paramref name="layer" />.
        /// </summary>
        public FilterableBindingBuilder<T> WhereLayerIs(int layer)
        {
            binding.AddFilter(o => GetGameObject(o)?.layer == layer);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is active in the hierarchy.
        /// </summary>
        public FilterableBindingBuilder<T> WhereActiveInHierarchy()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == true);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is inactive in the hierarchy.
        /// </summary>
        public FilterableBindingBuilder<T> WhereInactiveInHierarchy()
        {
            binding.AddFilter(o => GetGameObject(o)?.activeInHierarchy == false);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> has the given sibling index.
        /// </summary>
        public FilterableBindingBuilder<T> WhereSiblingIndexIs(int index)
        {
            binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == index);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the first sibling.
        /// </summary>
        public FilterableBindingBuilder<T> WhereIsFirstSibling()
        {
            binding.AddFilter(o => GetTransform(o)?.GetSiblingIndex() == 0);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the last sibling.
        /// </summary>
        public FilterableBindingBuilder<T> WhereIsLastSibling()
        {
            binding.AddFilter(o =>
            {
                Transform t = GetTransform(o);
                return t && t.parent && t.GetSiblingIndex() == t.parent.childCount - 1;
            });

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
    }
}