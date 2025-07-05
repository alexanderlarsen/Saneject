using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Represents a user-defined binding between an interface and a concrete type in Saneject's DI system.
    /// Encapsulates dependency location rules as defined by <see cref="Plugins.Saneject.Runtime.Bindings.ComponentBindingBuilder{T}" /> or <see cref="Plugins.Saneject.Runtime.Bindings.ObjectBindingBuilder{T}" />.
    /// </summary>
    public class Binding
    {
        public readonly Type interfaceType;
        public readonly Type concreteType;
        public readonly string id;
        private readonly List<Func<Object, bool>> filters = new();
        private readonly Func<Object, IEnumerable<Object>> locator;
        private readonly List<Func<Object, bool>> targetFilters = new();

        public Binding(
            Type interfaceType,
            Type concreteType,
            string id,
            Func<Object, IEnumerable<Object>> locator,
            bool requiresInjectionTarget = false)
        {
            this.interfaceType = interfaceType;
            this.concreteType = concreteType;
            this.id = id;
            this.locator = locator;
            RequiresInjectionTarget = requiresInjectionTarget;
        }

        public bool RequiresInjectionTarget { get; }
        public bool IsUsed { get; private set; }

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
        public Object LocateDependency(Object target = null)
        {
            IsUsed = true;

            if (targetFilters.Count > 0 && target != null && !targetFilters.All(f => f(target)))
                return null;

            if (RequiresInjectionTarget && target == null)
                return null;

            IEnumerable<Object> all = locator(target) ?? Enumerable.Empty<Object>();
            IEnumerable<Object> filtered = all.Where(obj => obj != null && filters.All(filter => filter(obj)));

            return filtered.FirstOrDefault();
        }
    }
}