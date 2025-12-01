using System;
using System.Collections.Generic;
using UnityEditor;

namespace Plugins.Saneject.Editor.Inspectors.API
{
    public struct PropertyData
    {
        public PropertyData(
            SerializedProperty property,
            string displayName,
            bool isReadOnly,
            bool isSerializedInterface,
            Type expectedType,
            List<PropertyData> children = null)
        {
            Property = property;
            DisplayName = displayName;
            IsReadOnly = isReadOnly;
            IsSerializedInterface = isSerializedInterface;
            ExpectedType = expectedType;
            Children = children;
        }

        public SerializedProperty Property { get; }
        public string DisplayName { get; }
        public bool IsReadOnly { get; }
        public bool IsSerializedInterface { get; }
        public Type ExpectedType { get; }
        public IReadOnlyList<PropertyData> Children { get; }
    }
}