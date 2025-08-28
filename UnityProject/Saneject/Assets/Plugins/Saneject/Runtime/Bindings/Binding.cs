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
        private readonly List<Func<Object, bool>> filters = new();
        private readonly List<(Func<Object, bool> qualifier, Type targetType)> injectionTargetQualifiers = new();
        private readonly List<(Func<string, bool> qualifier, string memberName)> injectionTargetMemberQualifiers = new();
        private readonly List<(Func<string, bool> qualifier, string id)> idQualifiers = new();

        private Func<Object, IEnumerable<Object>> locator;

        public Binding(
            Type interfaceType,
            Type concreteType,
            Scope scope)
        {
            Scope = scope;
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
        }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }

        public Scope Scope { get; }
        public bool HasLocator => locator != null;
        public string[] Ids => idQualifiers?.Select(q => q.id).ToArray();
        public bool IsGlobal { get; private set; }
        public bool RequiresInjectionTarget { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsCollection { get; private set; }
        public bool IsComponentBinding { get; private set; }
        public bool IsAssetBinding { get; private set; }
        public bool IsProxyBinding { get; private set; }

        /// <summary>
        /// Constructs a readable binding name used by the <c>DependencyInjector</c> for logging purposes,
        /// including interface and concrete type names and an optional ID.
        /// </summary>
        /// <summary>
        /// Marks this binding as eligible for collection injection, allowing it to be resolved into array or list fields.
        /// </summary>
        public void MarkCollectionBinding()
        {
            if (IsCollection)
            {
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked as a collection binding. Ignoring this call.", Scope);
                return;
            }

            IsCollection = true;
        }

        public void MarkComponentBinding()
        {
            if (IsComponentBinding)
            {
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked as a component binding. Ignoring this call.", Scope);
                return;
            }

            IsComponentBinding = true;
        }

        public void MarkAssetBinding()
        {
            if (IsAssetBinding)
            {
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked as an asset binding. Ignoring this call.", Scope);
                return;
            }

            IsAssetBinding = true;
        }

        /// <summary>
        /// Marks this binding as a global singleton, making it eligible for use in <see cref="Plugins.Saneject.Runtime.Global.GlobalScope" /> resolution by adding it to a <see cref="Plugins.Saneject.Runtime.Global.SceneGlobalContainer" />.
        /// </summary>
        public void MarkGlobal()
        {
            if (IsGlobal)
            {
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked as global. Ignoring this call.", Scope);
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
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked as requiring an injection target. Ignoring this call.", Scope);
                return;
            }

            RequiresInjectionTarget = true;
        }

        /// <summary>
        /// Tells the DI system to resolve this binding with a ProxyObject. The ProxyObject script and asset will be created at injection time, if they don't already exist. If they exist, the first found asset instance will be used.
        /// </summary>
        public void MarkResolveWithProxy()
        {
            if (IsProxyBinding)
            {
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} is already marked to resolve from proxy. Ignoring this call.", Scope);
                return;
            }

            IsProxyBinding = true;
        }

        /// <summary>
        /// Called by the <c>DependencyInjector</c> to mark the binding as used when consumed, so the system can log unused bindings after injection.
        /// </summary>
        public void MarkUsed()
        {
            IsUsed = true;
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
                Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} already has a locator and cannot specify multiple locators. Ignoring this call.", Scope);
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
        /// Adds a target-type qualifier for this binding.
        /// </summary>
        /// <param name="qualifier">Predicate to evaluate the injection target object.</param>
        /// <param name="targetType">Target type used for equality checks.</param>
        public void AddInjectionTargetQualifier(
            Func<Object, bool> qualifier,
            Type targetType)
        {
            injectionTargetQualifiers.Add((qualifier, targetType));
        }

        /// <summary>
        /// Adds a member-name qualifier for this binding.
        /// </summary>
        /// <param name="qualifier">Predicate to evaluate the member name.</param>
        /// <param name="memberName">Member name used for equality checks.</param>
        public void AddInjectionTargetMemberQualifier(
            Func<string, bool> qualifier,
            string memberName)
        {
            injectionTargetMemberQualifiers.Add((qualifier, memberName));
        }

        /// <summary>
        /// Adds an ID qualifier to restrict this binding by injection site ID.
        /// </summary>
        /// <param name="qualifier">Predicate that evaluates the injection site's ID string.</param>
        /// <param name="id">The raw ID string for equality and duplicate checks.</param>
        public void AddIdQualifier(
            Func<string, bool> qualifier,
            string id)
        {
            foreach (string existingId in idQualifiers.Select(q => q.id))
                if (existingId == id)
                {
                    Debug.LogWarning($"Saneject: Binding {this.GetBindingSignature()} already has an ID '{existingId}' and cannot have multiple identical IDs. Ignoring attempt to set ID to '{id}'.", Scope);
                    return;
                }

            idQualifiers.Add((qualifier, id));
        }

        /// <summary>
        /// Locate and return the dependency <see cref="UnityEngine.Object" />, applying all filters. Returns <c>null</c> if no suitable object is found.
        /// </summary>
        /// <param name="injectionTarget">
        /// The injection target <see cref="UnityEngine.Object" /> to evaluate target filters against (optional; required if <see cref="RequiresInjectionTarget" /> is true).
        /// </param>
        /// <returns>The resolved dependency <see cref="UnityEngine.Object" />, or <c>null</c> if not found.</returns>
        public IEnumerable<Object> LocateDependencies(Object injectionTarget = null)
        {
            IEnumerable<Object> all = locator(injectionTarget) ?? Enumerable.Empty<Object>();
            IEnumerable<Object> filtered = all.Where(obj => obj != null && filters.All(filter => filter(obj)));
            return filtered;
        }

        /// <summary>
        /// Checks if this binding's target filters pass for the given injection target.
        /// </summary>
        /// <param name="target">The injection target to evaluate against target filters.</param>
        /// <returns>True if all target filters pass or no target filters exist, false otherwise.</returns>
        public bool PassesInjectionTargetQualifiers(Object target)
        {
            if (injectionTargetQualifiers.Count == 0)
                return true;

            return target != null && injectionTargetQualifiers.Any(f => f.qualifier(target));
        }

        /// <summary>
        /// Checks if this binding's member-name qualifiers pass for the given name.
        /// </summary>
        /// <param name="targetMemberName">Field or property name on the injection target.</param>
        /// <returns>True if any qualifier matches or none exist, false otherwise.</returns>
        public bool PassesMemberNameQualifiers(string targetMemberName)
        {
            if (injectionTargetMemberQualifiers.Count == 0)
                return true;

            return !string.IsNullOrWhiteSpace(targetMemberName) && injectionTargetMemberQualifiers.Any(f => f.qualifier(targetMemberName));
        }

        /// <summary>
        /// Checks if this binding's ID qualifiers pass for the given ID.
        /// </summary>
        /// <param name="id">The injection site ID string.</param>
        /// <returns>True if any qualifier matches or none exist, false otherwise.</returns>
        public bool PassesIdQualifiers(string id)
        {
            if (idQualifiers.Count == 0)
                return string.IsNullOrWhiteSpace(id);

            return !string.IsNullOrWhiteSpace(id) && idQualifiers.Any(f => f.qualifier(id));
        }

        /// <summary>
        /// Custom equality comparison to determine if the binding is unique or a duplicate based on system rules.
        /// </summary>
        // Equals (replace your current implementation)
        public bool Equals(Binding other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Core signature
            bool typeMatch = InterfaceType != null
                ? InterfaceType == other.InterfaceType
                : ConcreteType == other.ConcreteType;

            if (!(Equals(Scope, other.Scope)
                  && typeMatch
                  && IsGlobal == other.IsGlobal
                  && IsCollection == other.IsCollection))
                return false;

            // Overlap-based equality for qualifiers
            IEnumerable<Type> myTargetTypes = injectionTargetQualifiers.Select(tf => tf.targetType);
            IEnumerable<Type> otherTargetTypes = other.injectionTargetQualifiers.Select(tf => tf.targetType);

            IEnumerable<string> myMemberNames = injectionTargetMemberQualifiers.Select(f => f.memberName);
            IEnumerable<string> otherMemberNames = other.injectionTargetMemberQualifiers.Select(f => f.memberName);

            IEnumerable<string> myIds = idQualifiers.Select(f => f.id);
            IEnumerable<string> otherIds = other.idQualifiers.Select(f => f.id);

            return TargetsOverlapForEquality(myTargetTypes, otherTargetTypes)
                   && MemberNamesOverlapForEquality(myMemberNames, otherMemberNames)
                   && IdsOverlapForEquality(myIds, otherIds);
        }

        public override int GetHashCode()
        {
            // if this is an interface binding, hash only on InterfaceType,
            // otherwise hash on ConcreteType
            Type keyType = InterfaceType ?? ConcreteType;
            int hash = HashCode.Combine(Scope, keyType, IsGlobal, IsCollection);

            // To satisfy (Equals => same hash), don't hash the exact contents of qualifiers,
            // only whether they are empty or not. Collisions are fine.
            bool hasTargetQualifiers = injectionTargetQualifiers.Any();
            bool hasMemberQualifiers = injectionTargetMemberQualifiers.Any();
            bool hasIdQualifiers = idQualifiers.Any();

            hash = HashCode.Combine(hash,
                hasTargetQualifiers ? 1 : 0,
                hasMemberQualifiers ? 1 : 0,
                hasIdQualifiers ? 1 : 0);

            return hash;
        }

        private static bool TargetsOverlapForEquality(
            IEnumerable<Type> a,
            IEnumerable<Type> b)
        {
            // Empty means "no restriction" – do NOT treat as equal to non-empty.
            List<Type> aList = a?.Where(t => t != null).Distinct().ToList() ?? new List<Type>();
            List<Type> bList = b?.Where(t => t != null).Distinct().ToList() ?? new List<Type>();

            if (aList.Count == 0 || bList.Count == 0)
                return aList.Count == 0 && bList.Count == 0;

            foreach (Type t1 in aList)
                foreach (Type t2 in bList)
                    if (t1.IsAssignableFrom(t2) || t2.IsAssignableFrom(t1))
                        return true;

            return false;
        }

        private static bool MemberNamesOverlapForEquality(
            IEnumerable<string> a,
            IEnumerable<string> b)
        {
            HashSet<string> aSet = new(a?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
            HashSet<string> bSet = new(b?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);

            if (aSet.Count == 0 || bSet.Count == 0)
                return aSet.Count == 0 && bSet.Count == 0;

            foreach (string name in aSet)
                if (bSet.Contains(name))
                    return true;

            return false;
        }

        private static bool IdsOverlapForEquality(
            IEnumerable<string> a,
            IEnumerable<string> b)
        {
            HashSet<string> aSet = new(a?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
            HashSet<string> bSet = new(b?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>(), StringComparer.Ordinal);

            if (aSet.Count == 0 || bSet.Count == 0)
                return aSet.Count == 0 && bSet.Count == 0;

            foreach (string id in aSet)
                if (bSet.Contains(id))
                    return true;

            return false;
        }
    }
}