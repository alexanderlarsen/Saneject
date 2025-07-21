using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Extensions
{
    public static class SerializedPropertyExtensions
    {
        public static void SetCollection(
            this SerializedProperty serializedProperty,
            Object[] collection)
        {
            if (!serializedProperty.isArray)
            {
                Debug.LogError("Saneject: Cannot set serialized array on non-array property.");
                return;
            }

            serializedProperty.arraySize = collection.Length;

            for (int i = 0; i < collection.Length; i++)
                serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = collection[i];
        }

        public static void NullifyOrClearArray(this SerializedProperty serializedProperty)
        {
            if (serializedProperty.isArray)
                serializedProperty.ClearArray();
            else
                serializedProperty.objectReferenceValue = null; // Nullify field
        }

        /// <summary>
        /// Navigates property path and returns the corresponding <see cref="FieldInfo" /> (if present).
        /// </summary>
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            string[] parts = property.propertyPath.Split('.');
            Type currentType = property.serializedObject.targetObject.GetType();
            FieldInfo field = null;

            foreach (string part in parts)
            {
                field = GetFieldInClassHierarchy(currentType, part);

                if (field == null)
                    return null;

                currentType = field.FieldType;
            }

            return field;
        }

        /// <summary>
        /// Returns the <see cref="FieldInfo" /> for a field with the given name, searching the entire class hierarchy (base types included).
        /// </summary>
        private static FieldInfo GetFieldInClassHierarchy(
            Type type,
            string fieldName)
        {
            while (type != null && type != typeof(object))
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null)
                    return field;

                type = type.BaseType;
            }

            return null;
        }
    }
}