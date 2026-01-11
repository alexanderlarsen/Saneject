using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Runtime.Attributes
{
    /// <summary>
    /// Marks a field or method for dependency injection by Saneject.
    /// When applied, the member will be resolved by the DI system and shown as read-only in the Unity editor.
    /// </summary>
    /// <remarks>
    /// Use this attribute to declare which fields or methods should receive dependencies from a <see cref="Scopes.Scope" />.
    /// During an injection pass, Saneject automatically resolves and assigns matching bindings defined in the nearest active scope.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class InjectAttribute : PropertyAttribute
    {
        /// <summary>
        /// Marks the field or method for injection without an ID.
        /// </summary>
        /// <remarks>
        /// The field or method will be resolved using the first matching binding of its type
        /// found in the current or any parent <see cref="Scopes.Scope" />.
        /// </remarks>
        public InjectAttribute()
        {
        }

        /// <summary>
        /// Marks the field or method for injection using a specific binding ID.
        /// </summary>
        /// <param name="id">
        /// The binding ID used to match this injection site against a binding declared in a <see cref="Scopes.Scope" />.
        /// Only bindings that declare the same ID will be used for resolution.
        /// </param>
        public InjectAttribute(string id)
        {
            ID = id;
        }

        /// <summary>
        /// Marks the field or method for injection, optionally suppressing missing-binding and missing-dependency errors.
        /// </summary>
        /// <param name="suppressMissingErrors">
        /// If <c>true</c>, Saneject will not log errors or warnings when no binding or dependency is found for this field or method parameter.
        /// Intended for advanced or special-case scenarios where certain dependencies are intentionally left unresolved.
        /// Missing dependency and binding errors are still counted and noted below the injection summary log.
        /// </param>
        public InjectAttribute(bool suppressMissingErrors)
        {
            ID = null;
            SuppressMissingErrors = suppressMissingErrors;
        }

        /// <summary>
        /// Marks the field or method for injection with both a binding ID and optional error suppression.
        /// </summary>
        /// <param name="id">The injection ID used for resolving the dependency.</param>
        /// <param name="suppressMissingErrors">
        /// If <c>true</c>, Saneject will not log errors or warnings when no binding or dependency is found for this field or method parameter.
        /// Intended for advanced or special-case scenarios where certain dependencies are intentionally left unresolved.
        /// Missing dependency and binding errors are still counted and noted below the injection summary log.
        /// </param>
        public InjectAttribute(
            string id,
            bool suppressMissingErrors)
        {
            ID = id;
            SuppressMissingErrors = suppressMissingErrors;
        }

        /// <summary>
        /// The binding ID used for matching against a <see cref="Scopes.Scope" /> binding.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// If <c>true</c>, Saneject suppresses missing-binding and missing-dependency error logs for this member.
        /// Such missing references are still counted and noted below the injection summary log.
        /// </summary>
        public bool SuppressMissingErrors { get; }
    }
}