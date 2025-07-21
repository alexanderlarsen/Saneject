using UnityEditor;
using UnityEngine;

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

        public static void NullifyOrClear(this SerializedProperty serializedProperty)
        {
            if (serializedProperty.isArray)
                serializedProperty.ClearArray();
            else
                serializedProperty.objectReferenceValue = null; // Nullify field
        }
    }
}