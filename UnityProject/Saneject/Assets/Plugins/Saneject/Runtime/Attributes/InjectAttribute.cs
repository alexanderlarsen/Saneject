using System;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Attributes
{
    /// <summary>
    /// Marks a field for dependency injection by Saneject.
    /// When applied, the field will be resolved by the DI system and shown as read-only in the Unity editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : PropertyAttribute
    {
        /// <summary>
        /// Marks the field for injection without an ID.
        /// </summary>
        public InjectAttribute()
        {
            HasId = false;
        }

        /// <summary>
        /// Marks the field for injection with an ID. Only bindings with the same ID in a <see cref="Scope" /> will be used to resolve this dependency.
        /// </summary>
        /// <param name="id">The injection ID used for resolving the dependency.</param>
        public InjectAttribute(string id)
        {
            ID = id;
            HasId = true;
        }

        public string ID { get; }
        public bool HasId { get; }
    }
}