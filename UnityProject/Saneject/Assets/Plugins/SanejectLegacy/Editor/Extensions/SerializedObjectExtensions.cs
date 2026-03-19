using UnityEditor;

namespace Plugins.SanejectLegacy.Editor.Extensions
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