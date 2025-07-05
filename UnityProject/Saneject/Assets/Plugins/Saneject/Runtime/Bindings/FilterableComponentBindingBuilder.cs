using System;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Fluent builder for adding filters to a component binding created by <see cref="ComponentBindingBuilder{T}" />.
    /// Enables fine-grained selection of <typeparamref name="T" /> (a <see cref="Component"/>) by <see cref="GameObject"/> name, tag, layer, active state, sibling index, or custom predicates.
    /// </summary>
    /// <typeparam name="T">The <see cref="Component" /> type to filter.</typeparam>
    public class FilterableComponentBindingBuilder<T> where T : Component
    {
        private readonly Binding binding;

        public FilterableComponentBindingBuilder(Binding binding)
        {
            this.binding = binding;
        }

        /// <summary>
        /// Filter to only resolve dependencies when the injection target is of type <typeparamref name="TTarget" />.
        /// Injection target is the <see cref="Component" /> of a field/property marked with <see cref="Plugins.Saneject.Runtime.Attributes.InjectAttribute" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereTargetIs<TTarget>()
        {
            binding.AddTargetFilter(target => target is TTarget);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.tag" /> matches <paramref name="tag" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereTagIs(string tag)
        {
            binding.AddFilter(o => o is Component c && c.gameObject.CompareTag(tag));
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> matches <paramref name="name" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereNameIs(string name)
        {
            binding.AddFilter(o => o is Component c && c.gameObject.name == name);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.name" /> contains <paramref name="substring" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereNameContains(string substring)
        {
            binding.AddFilter(o => o is Component c && c.gameObject.name.Contains(substring));
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> <see cref="GameObject.layer" /> matches <paramref name="layer" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereLayerIs(int layer)
        {
            binding.AddFilter(o => o is Component c && c.gameObject.layer == layer);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is active in the hierarchy.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereActiveInHierarchy()
        {
            binding.AddFilter(o => o is Component { gameObject: { activeInHierarchy: true } });
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="GameObject" /> is inactive in the hierarchy.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereInactiveInHierarchy()
        {
            binding.AddFilter(o => o is Component { gameObject: { activeInHierarchy: false } });
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> has the given sibling index.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereSiblingIndexIs(int index)
        {
            binding.AddFilter(o => o is Component c && c.transform.GetSiblingIndex() == index);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the first sibling.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereIsFirstSibling()
        {
            binding.AddFilter(o => o is Component c && c.transform.GetSiblingIndex() == 0);
            return this;
        }

        /// <summary>
        /// Filter to only include <see cref="Component" />s whose <see cref="Transform" /> is the last sibling.
        /// </summary>
        public FilterableComponentBindingBuilder<T> WhereIsLastSibling()
        {
            binding.AddFilter(o =>
            {
                if (o is not Component c) return false;
                Transform parent = c.transform.parent;
                return parent && c.transform.GetSiblingIndex() == parent.childCount - 1;
            });

            return this;
        }

        /// <summary>
        /// Filter using a custom predicate on <typeparamref name="T" />.
        /// </summary>
        public FilterableComponentBindingBuilder<T> Where(Func<T, bool> predicate)
        {
            binding.AddFilter(o => o is T t && predicate(t));
            return this;
        }
    }
}