using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Inspectors.Models
{
    /// <summary>
    /// Encapsulates a target component and its inspectable properties for drawing in the custom Saneject inspector.
    /// This model reflects on the target object's type to collect all drawable fields and constructs
    /// PropertyModel instances for each field, handling serialized interfaces, nested types, and
    /// injection-aware properties.
    /// </summary>
    public sealed class ComponentModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentModel"/> class.
        /// </summary>
        /// <param name="target">The target object being inspected. Used for type reflection.</param>
        /// <param name="serializedObject">The <see cref="SerializedObject"/> representing the target for property access.</param>
        public ComponentModel(
            Object target,
            SerializedObject serializedObject)
        {
            SerializedObject = serializedObject;
            Target = target;

            Properties = target
                .GetType()
                .GetFieldInfosForInspector()
                .Select(field => new PropertyModel(serializedObject, field))
                .Where(p => p.SerializedProperty != null)
                .ToList();
        }

        /// <summary>
        /// Gets the target object being inspected.
        /// </summary>
        public Object Target { get; }

        /// <summary>
        /// Gets the serialized representation of the target object.
        /// </summary>
        public SerializedObject SerializedObject { get; }

        /// <summary>
        /// Gets the read-only collection of inspectable properties of the target.
        /// </summary>
        public IReadOnlyList<PropertyModel> Properties { get; }
    }
}