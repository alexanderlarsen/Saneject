using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Attributes;
using UnityEditor;

namespace Plugins.Saneject.Editor.Inspectors.Models
{
    /// <summary>
    /// Encapsulates metadata and state for a serializable property in the custom Saneject inspector.
    /// This model combines reflection information about a field with its serialized property representation,
    /// handling special cases like <see cref="SerializeInterfaceAttribute" /> fields, nested serializable types, injected fields,
    /// and read-only properties. It supports hierarchical property structures for nested types and collections.
    /// </summary>
    public sealed class PropertyModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyModel"/> class.
        /// </summary>
        /// <param name="root">The root <see cref="SerializedObject" /> used to find top-level properties.</param>
        /// <param name="field">The FieldInfo representing the property.</param>
        /// <param name="parent">
        /// The parent <see cref="SerializedProperty" /> if this property is nested within another serializable type.
        /// If null, the property is resolved from the root <see cref="SerializedObject" />.
        /// </param>
        public PropertyModel(
            SerializedObject root,
            FieldInfo field,
            SerializedProperty parent = null)
        {
            bool isSerializedInterface = field.IsSerializeInterface();
            Type elementType = field.FieldType.ResolveElementType();

            string displayName = ObjectNames.NicifyVariableName(field.Name);
            string path = field.Name;

            if (isSerializedInterface)
            {
                displayName += $" ({elementType.Name})";
                path = $"__{NameUtility.GetLogicalName(field.Name)}";
            }

            SerializedProperty = parent == null
                ? root.FindProperty(path)
                : parent.FindPropertyRelative(path);

            DisplayName = displayName;
            IsSerializedInterface = isSerializedInterface;
            ExpectedType = elementType;
            HasInjectAttribute = field.HasInjectAttribute();
            IsReadOnly = HasInjectAttribute || field.HasReadOnlyAttribute();
            IsCollection = SerializedProperty.isArray && SerializedProperty.propertyType != SerializedPropertyType.String;

            Children = field.FieldType.IsNestedSerializable() && SerializedProperty != null
                ? field.FieldType
                    .GetFieldInfosForInspector()
                    .Select(child => new PropertyModel(root, child, SerializedProperty))
                    .Where(p => p.SerializedProperty != null)
                    .ToList()
                : new List<PropertyModel>();
        }

        /// <summary>
        /// Gets the serialized property representation of this field.
        /// </summary>
        public SerializedProperty SerializedProperty { get; }

        /// <summary>
        /// Gets the display name for this property in the inspector, nicified and with type information
        /// for <see cref="SerializeInterfaceAttribute" /> fields.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets a value indicating whether this property should be drawn as read-only in the inspector.
        /// This is true if the field is marked with <see cref="InjectAttribute" />  or <see cref="ReadOnlyAttribute" /> .
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        /// Gets a value indicating whether this property is marked with the <see cref="SerializeInterfaceAttribute" /> attribute.
        /// </summary>
        public bool IsSerializedInterface { get; }

        /// <summary>
        /// Gets the expected element type for this property.
        /// For <see cref="SerializeInterfaceAttribute" /> fields, this is the interface type.
        /// For non-interface properties, this is the field type itself.
        /// </summary>
        public Type ExpectedType { get; }

        /// <summary>
        /// Gets a value indicating whether this property is marked with the <see cref="InjectAttribute" /> attribute.
        /// </summary>
        public bool HasInjectAttribute { get; }

        /// <summary>
        /// Gets a value indicating whether this property is an array or list type.
        /// </summary>
        public bool IsCollection { get; }

        /// <summary>
        /// Gets the read-only collection of child PropertyModel instances for nested serializable types.
        /// Empty if this property is not a nested serializable type or if no children are found.
        /// </summary>
        public IReadOnlyList<PropertyModel> Children { get; }
    }
}