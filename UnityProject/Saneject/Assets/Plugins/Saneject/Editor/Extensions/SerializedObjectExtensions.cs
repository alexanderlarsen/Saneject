﻿using UnityEditor;

namespace Plugins.Saneject.Editor.Extensions
{
    public static class SerializedObjectExtensions
    {
        public static void Save(this SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
    }
}