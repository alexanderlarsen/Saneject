using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks a field or method for dependency injection by Saneject.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to a field or method to declare that it should receive a dependency from the nearest upwards scope.
    /// During injection, Saneject resolves matching bindings from the scope hierarchy and automatically assigns or invokes the member.
    /// Injected fields are displayed as read-only in the inspector.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class InjectAttribute : PropertyAttribute
    {
        /// <summary>
        /// Marks the field or method for injection, using only type-based binding resolution.
        /// </summary>
        /// <remarks>
        /// The dependency is resolved by matching the member's type against bindings in the scope hierarchy.
        /// </remarks>
        public InjectAttribute()
        {
        }

        /// <summary>
        /// Marks the field or method for injection using a specific binding ID.
        /// </summary>
        /// <param name="id">The binding ID to match against. Only bindings with the same ID will be used for resolution.</param>
        public InjectAttribute(string id)
        {
            ID = id;
        }

        /// <summary>
        /// Marks the field or method as an optional injection target.
        /// </summary>
        /// <param name="suppressMissingErrors">
        /// If <c>true</c>, suppresses error logs when no binding or dependency is found.
        /// </param>
        public InjectAttribute(bool suppressMissingErrors)
        {
            ID = null;
            SuppressMissingErrors = suppressMissingErrors;
        }

        /// <summary>
        /// Marks the field or method for injection with an ID and optional error suppression.
        /// </summary>
        /// <param name="id">The binding ID to match against.</param>
        /// <param name="suppressMissingErrors">
        /// If <c>true</c>, suppresses error logs when no binding or dependency is found.
        /// </param>
        public InjectAttribute(
            string id,
            bool suppressMissingErrors)
        {
            ID = id;
            SuppressMissingErrors = suppressMissingErrors;
        }

        /// <summary>
        /// Gets the binding ID used for dependency resolution, or <c>null</c> if only type-based resolution is used.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Gets a value indicating whether missing binding and dependency errors are suppressed for this injection site.
        /// </summary>
        public bool SuppressMissingErrors { get; }
    }
}