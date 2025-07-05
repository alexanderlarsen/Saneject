using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="InjectAttribute"/>.
    /// Displays injected fields in the inspector as read-only, unless hidden by user settings.
    /// </summary>
    [CustomPropertyDrawer(typeof(InjectAttribute))]
    public class InjectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            if (!UserSettings.ShowInjectedFields)
                return;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return UserSettings.ShowInjectedFields
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;

        }
    }
}