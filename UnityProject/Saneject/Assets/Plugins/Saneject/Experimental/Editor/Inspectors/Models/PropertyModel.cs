using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Inspectors.Models
{
    public sealed class PropertyModel
    {
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
                path = $"__{BackingFieldNameUtility.GetLogicalName(field.Name)}";
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

        public SerializedProperty SerializedProperty { get; }
        public string DisplayName { get; }
        public bool IsReadOnly { get; }
        public bool IsSerializedInterface { get; }
        public Type ExpectedType { get; }
        public bool HasInjectAttribute { get; }
        public bool IsCollection { get; }
        public IReadOnlyList<PropertyModel> Children { get; }
    }
}