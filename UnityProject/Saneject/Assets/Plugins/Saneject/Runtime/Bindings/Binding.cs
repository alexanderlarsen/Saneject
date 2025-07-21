using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SetId(string id)
        {
            Id = id;
        }

        public void MarkGlobal()
        {
            IsGlobal = true;
        }

        public void MarkRequireInjectionTarget()
        {
            RequiresInjectionTarget = true;
        }

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