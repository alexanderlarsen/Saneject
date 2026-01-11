using System;
using System.Collections.Generic;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Inspectors.API
{
    public sealed class FieldData
    {
        public FieldData(
            SerializedProperty property,
            string displayName,
            bool isReadOnly,
            bool isSerializedInterface,
            Type expectedType,
            List<FieldData> children)
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
        public IReadOnlyList<FieldData> Children { get; }
    }
}