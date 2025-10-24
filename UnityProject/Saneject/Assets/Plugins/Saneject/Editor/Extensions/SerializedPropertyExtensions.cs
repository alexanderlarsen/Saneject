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

        public static void Clear(this SerializedProperty serializedProperty)
        {
            if (serializedProperty.isArray)
                serializedProperty.ClearArray();
            else
                serializedProperty.objectReferenceValue = null; // Nullify field/property
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
        /// Gets the Type that declares the field represented by this SerializedProperty.
        /// For nested serialized classes, returns the nested class type, not the outer MonoBehaviour.
        /// </summary>
        public static Type GetDeclaringType(
            this SerializedProperty property,
            Object targetObject)
        {
            if (property == null || targetObject == null)
                return null;

            string path = property.propertyPath;
            Type currentType = targetObject.GetType();

            // Split path by '.' but handle array indices
            string[] pathParts = path.Replace(".Array.data[", "[").Split('.');

            // Navigate to the declaring type (all parts except the last one, which is the field name)
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                string part = pathParts[i];

                // Handle array indexing
                if (part.Contains("["))
                {
                    string fieldName = part[..part.IndexOf('[')];

                    if (currentType == null)
                        continue;

                    FieldInfo arrayField = currentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (arrayField != null)
                    {
                        Type elementType = arrayField.FieldType.IsArray
                            ? arrayField.FieldType.GetElementType()
                            : arrayField.FieldType.IsGenericType
                                ? arrayField.FieldType.GetGenericArguments()[0]
                                : arrayField.FieldType;

                        currentType = elementType;
                    }
                }
                else
                {
                    if (currentType == null)
                        continue;

                    FieldInfo field = currentType.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field != null)
                        currentType = field.FieldType;
                    else
                        break;
                }
            }

            return currentType;
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