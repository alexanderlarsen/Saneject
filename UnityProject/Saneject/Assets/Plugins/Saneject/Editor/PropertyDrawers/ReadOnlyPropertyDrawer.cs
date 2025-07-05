using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.PropertyDrawers
{
    /// <summary>
    /// Draws properties marked with <see cref="ReadOnlyAttribute" /> as disabled in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(Plugins.Saneject.Runtime.Attributes.ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}