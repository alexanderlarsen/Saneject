using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBeConvertedToQuery

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Plugins.Saneject.Runtime.Bindings
{
    /// <summary>
    /// Represents a user-defined binding between an interface and a concrete type in Saneject's DI system.
    /// Encapsulates dependency location rules as defined by <see cref="ComponentBindingBuilder{T}" />, <see cref="AssetBindingBuilder{T}" />, <see cref="ComponentFilterBuilder{T}" />, <see cref="AssetFilterBuilder{T}" />.
    /// </summary>
    public class Binding : IEquatable<Binding>
    {
        private readonly Scope scope;
        private readonly List<Func<Object, bool>> filters = new();
        private readonly List<(Func<Object, bool> filter, Type targetType)> targetFilters = new();

        private Func<Object, IEnumerable<Object>> locator;

        public Binding(
            Type interfaceType,
            Type concreteType,
            Scope scope)
        {
            this.scope = scope;
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
        }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public string Id { get; private set; }
        public bool IsGlobal { get; private set; }
        public bool RequiresInjectionTarget { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsCollection { get; private set; }
        public bool IsComponentBinding { get; private set; }
        public bool IsAssetBinding { get; private set; }

        /// <summary>
        /// Constructs a readable binding name used by the <c>DependencyInjector</c> for logging purposes,
        /// including interface and concrete type names and an optional ID.
        /// </summary>
        public static string ConstructBindingName(
            Type interfaceType,
            Type concreteType,
            string id)
        {
            string output = string.Empty;

            if (interfaceType != null && concreteType != null)
                output = $"{interfaceType.Name} -> {concreteType.Name}";
            else if (interfaceType == null && concreteType != null)
                output = $"{concreteType.Name}";
            else if (interfaceType != null)
                output = $"{interfaceType.Name}";

            if (id != null)
                output += $" | ID: {id}";

            return output;
        }

        /// <summary>
        /// Marks this binding as eligible for collection injection, allowing it to be resolved into array or list fields.
        /// </summary>
        public void MarkCollectionBinding()
        {
            if (IsCollection)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is already marked as a collection binding. Ignoring this call.", scope);
                return;
            }

            IsCollection = true;
        }

        public void MarkComponentBinding()
        {
            if (IsComponentBinding)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is already marked as a component binding. Ignoring this call.", scope);
                return;
            }

            IsComponentBinding = true;
        }

        public void MarkAssetBinding()
        {
            if (IsAssetBinding)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is already marked as an asset binding. Ignoring this call.", scope);
                return;
            }

            IsAssetBinding = true;
        }

        /// <summary>
        /// Sets a custom ID for this binding to match against fields marked with <c>[Inject(Id = "...")]</c>.
        /// </summary>
        /// <param name="newId">The custom ID string.</param>
        public void SetId(string newId)
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' already has an ID '{Id}' and cannot have multiple IDs. Ignoring attempt to set ID to '{newId}'.", scope);
                return;
            }

            Id = newId;
        }

        /// <summary>
        /// Marks this binding as a global singleton, making it eligible for use in <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> resolution by adding it to a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" />.
        /// </summary>
        public void MarkGlobal()
        {
            if (IsGlobal)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is already marked as global. Ignoring this call.", scope);
                return;
            }

            IsGlobal = true;
        }

        /// <summary>
        /// Indicates that this binding must be resolved relative to an injection target <see cref="UnityEngine.Object" />.
        /// </summary>
        public void MarkRequireInjectionTarget()
        {
            if (RequiresInjectionTarget)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is already marked as requiring an injection target. Ignoring this call.", scope);
                return;
            }

            RequiresInjectionTarget = true;
        }

        /// <summary>
        /// Sets the method that locates candidate objects for dependency resolution.
        /// This delegate is invoked during injection passes.
        /// </summary>
        /// <param name="locator">A delegate that returns a collection of candidate <see cref="UnityEngine.Object" /> instances.</param>
        public void SetLocator(Func<Object, IEnumerable<Object>> locator)
        {
            if (this.locator != null)
            {
                Debug.LogWarning($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' already has a locator and cannot specify multiple locators. Ignoring this call.", scope);
                return;
            }

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
        /// <param name="targetType"></param>
        public void AddTargetFilter(
            Func<Object, bool> filter,
            Type targetType)
        {
            targetFilters.Add((filter, targetType));
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

            if (targetFilters.Count > 0 && target != null && !targetFilters.All(f => f.filter(target)))
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
            if (targetFilters.Count == 0)
                return true;

            return target != null && targetFilters.All(f => f.filter(target));
        }

        public bool IsValid()
        {
            bool isValid = true;

            if (IsComponentBinding && ConcreteType != null && !typeof(Component).IsAssignableFrom(ConcreteType))
            {
                string bindingType = $"{(IsCollection ? "Multiple component bindings" : "Component binding")}";

                Debug.LogError($"Saneject: {bindingType} in '{scope.GetType().Name}' has an invalid type '{ConcreteType.Name}' that is not an interface or component.", scope);

                isValid = false;
            }

            if (InterfaceType is { IsInterface: false })
            {
                Debug.LogError($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' has an invalid interface type '{InterfaceType.FullName}' that is not an interface.", scope);

                isValid = false;
            }

            if (IsGlobal && IsCollection)
            {
                Debug.LogError($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' is both global and collection. This is not allowed.", scope);

                isValid = false;
            }

            if (IsGlobal && !string.IsNullOrWhiteSpace(Id))
            {
                Debug.LogError($"Saneject: Global binding ({GetName()}) in scope '{scope.GetType().Name}' is not allowed to have an ID.", scope);

                isValid = false;
            }

            if (locator == null)
            {
                Debug.LogError($"Saneject: Binding ({GetName()}) in scope '{scope.GetType().Name}' does not have a locator (e.g., FromScopeSelf). This is required.", scope);

                isValid = false;
            }

            return isValid;
        }

        public string GetName()
        {
            return ConstructBindingName(InterfaceType, ConcreteType, Id);
        }

        public bool Equals(Binding other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // two bindings conflict if they have the same interface (regardless of concrete)
            // or, if neither has an interface, they must have the same concrete
            bool typeMatch;

            if (InterfaceType != null)
                typeMatch = InterfaceType == other.InterfaceType;
            else
                typeMatch = ConcreteType == other.ConcreteType;

            return Equals(scope, other.scope)
                   && typeMatch
                   && Id == other.Id
                   && IsGlobal == other.IsGlobal
                   && IsCollection == other.IsCollection
                   && targetFilters.Select(tf => tf.targetType)
                       .OrderBy(t => t?.FullName)
                       .SequenceEqual(other.targetFilters.Select(tf => tf.targetType)
                           .OrderBy(t => t?.FullName));
        }

        public override int GetHashCode()
        {
            // if this is an interface binding, hash only on InterfaceType,
            // otherwise hash on ConcreteType
            Type keyType = InterfaceType ?? ConcreteType;
            int hash = HashCode.Combine(scope, keyType, Id, IsGlobal, IsCollection);

            foreach (Type targetType in targetFilters.Select(tf => tf.targetType).OrderBy(t => t?.FullName))
                hash = HashCode.Combine(hash, targetType);

            return hash;
        }
    }
}