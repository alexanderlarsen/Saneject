using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Represents a user-defined binding between an interface and a concrete type in Saneject's DI system.
    /// Encapsulates dependency location rules as defined by <see cref="BindingBuilder{T}" />.
    /// </summary>
    public class Binding
    {
        private readonly List<Func<Object, bool>> filters = new();
        private readonly List<Func<Object, bool>> targetFilters = new();

        private Func<Object, IEnumerable<Object>> locator;

        public Binding(
            Type interfaceType,
            Type concreteType)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
        }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public string Id { get; private set; }
        public bool IsGlobal { get; private set; }
        public bool RequiresInjectionTarget { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsCollectionBinding { get; private set; }

        public void MarkCollectionBinding()
        {
            if (IsGlobal)
            {
                Debug.LogError("Saneject: Collection bindings cannot be marked as global.");
                return;
            }

            IsCollectionBinding = true;
        }

        /// <summary>
        /// Sets a custom ID for this binding to match against fields marked with <c>[Inject(Id = "...")]</c>.
        /// </summary>
        /// <param name="id">The custom ID string.</param>
        public void SetId(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Marks this binding as global, making it eligible for use in <c>GlobalScope</c> resolution.
        /// </summary>
        public void MarkGlobal()
        {
            if (IsCollectionBinding)
            {
                Debug.LogError("Saneject: Collection bindings cannot be marked as global.");
                return;
            }

            IsGlobal = true;
        }

        /// <summary>
        /// Indicates that this binding must be resolved relative to an injection target <see cref="UnityEngine.Object" />.
        /// </summary>
        public void MarkRequireInjectionTarget()
        {
            RequiresInjectionTarget = true;
        }

        /// <summary>
        /// Sets the method that locates candidate objects for dependency resolution.
        /// This delegate is invoked during injection passes.
        /// </summary>
        /// <param name="locator">A delegate that returns a collection of candidate <see cref="UnityEngine.Object" /> instances.</param>
        public void SetLocator(Func<Object, IEnumerable<Object>> locator)
        {
            this.locator = locator;
        }

        /// <summary>
        /// Add a filter for candidate <see cref="UnityEngine.Object" />s. Only objects passing all filters will be considered for resolution.
        /// </summary>
        /// <param name="filter">Predicate to evaluate each <see cref="UnityEngine.Object" />.</param>
        public void AddFilter(Func<Object, bool> filter)
        {
            filters.Add(filter);
        }

        /// <summary>
        /// Add a filter for injection targets. Only targets passing all target filters are valid for this binding.
        /// </summary>
        /// <param name="filter">Predicate to evaluate each injection target <see cref="UnityEngine.Object" />.</param>
        public void AddTargetFilter(Func<Object, bool> filter)
        {
            targetFilters.Add(filter);
        }

        /// <summary>
        /// Locate and return the dependency <see cref="UnityEngine.Object" />, applying all filters. Returns <c>null</c> if no suitable object is found.
        /// </summary>
        /// <param name="target">
        /// The injection target <see cref="UnityEngine.Object" /> to evaluate target filters against (optional; required if <see cref="RequiresInjectionTarget" /> is true).
        /// </param>
        /// <returns>The resolved dependency <see cref="UnityEngine.Object" />, or <c>null</c> if not found.</returns>
        public IEnumerable<Object> LocateDependencies(Object target = null)
        {
            IsUsed = true;

            if (targetFilters.Count > 0 && target != null && !targetFilters.All(f => f(target)))
                return null;

            if (RequiresInjectionTarget && target == null)
                return null;

            IEnumerable<Object> all = locator(target) ?? Enumerable.Empty<Object>();
            IEnumerable<Object> filtered = all.Where(obj => obj != null && filters.All(filter => filter(obj)));

            return filtered;
        }

        /// <summary>
        /// Checks if this binding's target filters pass for the given injection target.
        /// </summary>
        /// <param name="target">The injection target to evaluate against target filters.</param>
        /// <returns>True if all target filters pass or no target filters exist, false otherwise.</returns>
        public bool PassesTargetFilters(Object target)
        {
            // If no target filters, binding always passes
            if (targetFilters.Count == 0)
                return true;

            // If target filters exist but no target provided, binding fails
            if (target == null)
                return false;

            // Check if all target filters pass
            return targetFilters.All(f => f(target));
        }
    }
}