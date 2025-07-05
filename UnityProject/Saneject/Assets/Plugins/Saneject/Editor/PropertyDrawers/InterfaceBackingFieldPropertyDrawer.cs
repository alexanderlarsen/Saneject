using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="InterfaceBackingFieldAttribute"/>.
    /// Draws interface backing fields in the inspector, formats their label, disables editing if injected, and syncs their value from the interface property.
    /// </summary>
    [CustomPropertyDrawer(typeof(InterfaceBackingFieldAttribute))]
    public class InterfaceBackingFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            if (!UserSettings.ShowInjectedFields)
                return;

            InterfaceBackingFieldAttribute interfaceBackingFieldAttribute = (InterfaceBackingFieldAttribute)attribute;
            Type interfaceType = interfaceBackingFieldAttribute.InterfaceType;
            bool isInjected = interfaceBackingFieldAttribute.IsInjected;

            label.text = FormatLabel(property.name, interfaceType);

            if (!interfaceType.IsInterface)
            {
                DrawError(position, label);
                return;
            }

            SyncBackingFieldFromInterface(property);

            using (new EditorGUI.DisabledScope(isInjected))
            {
                DrawObjectField(position, property, label, interfaceType);
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

        private static void SyncBackingFieldFromInterface(SerializedProperty property)
        {
            string interfaceFieldName = GetInterfaceFieldName(property.name);
            string interfacePath = ReplaceLastPropertyPathElement(property.propertyPath, interfaceFieldName);

            SerializedProperty interfaceProp = property.serializedObject.FindProperty(interfacePath);

            if (interfaceProp == null)
                return;

            Object interfaceValue = interfaceProp.objectReferenceValue;

            if (interfaceValue != property.objectReferenceValue)
            {
                property.objectReferenceValue = interfaceValue;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static string ReplaceLastPropertyPathElement(
            string propertyPath,
            string newElement)
        {
            int lastDot = propertyPath.LastIndexOf('.');

            if (lastDot < 0)
                return newElement;

            return propertyPath[..(lastDot + 1)] + newElement;
        }

        private static string GetInterfaceFieldName(string backingFieldName)
        {
            return backingFieldName.StartsWith("__") ? backingFieldName[2..] : backingFieldName;
        }

        private static void DrawObjectField(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            Type interfaceType)
        {
            EditorGUI.BeginProperty(position, label, property);

            Object oldValue = property.objectReferenceValue;
            Object newValue = EditorGUI.ObjectField(position, label, oldValue, typeof(Object), true);

            if (newValue != null && !interfaceType.IsAssignableFrom(newValue.GetType()))
            {
                Debug.LogError($"Saneject: '{newValue.GetType().Name}' does not implement {interfaceType.Name}", newValue);
                newValue = null;
            }

            property.objectReferenceValue = newValue;

            EditorGUI.EndProperty();
        }

        private static void DrawError(
            Rect position,
            GUIContent label)
        {
            EditorGUI.HelpBox(position, $"Unable to resolve interface type for {label.text}", MessageType.Error);
        }

        private static string FormatLabel(
            string fieldName,
            Type interfaceType)
        {
            string trimmed = fieldName.TrimStart('_');
            string spaced = Regex.Replace(trimmed, "([a-z])([A-Z])", "$1 $2");
            TextInfo ti = CultureInfo.InvariantCulture.TextInfo;
            string titled = ti.ToTitleCase(spaced);
            return $"{titled} ({interfaceType.Name})";
        }
    }
}