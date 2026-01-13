using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Inspectors.Models;
using Plugins.Saneject.Experimental.Runtime.Attributes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Inspectors
{
    public static class SanejectInspector
    {
        #region Drawing

        /// <summary>
        /// Draws the complete default Saneject MonoBehaviour inspector, including the script field, all serializable fields in declaration order, injection-aware read-only handling, custom UI and validation for [SerializeInterface] fields, and recursive drawing of nested serializable types.
        /// </summary>
        public static void OnInspectorGUI(ComponentModel componentModel)
        {
            componentModel.SerializedObject.Update();
            DrawMonoBehaviourScriptField(componentModel.Target);

            foreach (PropertyModel propertyData in componentModel.Properties)
            {
                DrawProperty(propertyData);
                ValidateProperty(propertyData);
            }

            componentModel.SerializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the default script field at the top of a MonoBehaviour inspector.
        /// </summary>
        public static void DrawMonoBehaviourScriptField(Object target)
        {
            if (target is not MonoBehaviour mono)
                return;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mono), typeof(MonoScript), false);
            }
        }

        /// <summary>
        /// Draws a single <see cref="model" /> with the given display name, read-only flag and validates interface fields, including nested serializable types.
        /// </summary>
        /// <param name="model"></param>
        public static void DrawProperty(PropertyModel model)
        {
            if (model.HasInjectAttribute && !UserSettings.ShowInjectedFieldsProperties)
                return;

            if (model.Children.Count == 0)
            {
                using (new EditorGUI.DisabledScope(model.IsReadOnly))
                {
                    EditorGUILayout.PropertyField
                    (
                        model.SerializedProperty,
                        new GUIContent(model.DisplayName),
                        true
                    );
                }

                return;
            }

            model.SerializedProperty.isExpanded = EditorGUILayout.Foldout
            (
                model.SerializedProperty.isExpanded,
                new GUIContent(model.DisplayName),
                true,
                EditorStyles.foldoutHeader
            );

            if (!model.SerializedProperty.isExpanded)
                return;

            EditorGUI.indentLevel++;

            foreach (PropertyModel child in model.Children)
                DrawProperty(child);

            EditorGUI.indentLevel--;
        }

        #endregion
        
        #region Validation

        /// <summary>
        /// Validates that the field is assigned to an object that implements the expected type.
        /// </summary>
        public static void ValidateProperty(PropertyModel property)
        {
            if (property.IsSerializedInterface)
            {
                if (property.IsCollection)
                    for (int i = 0; i < property.SerializedProperty.arraySize; i++)
                    {
                        SerializedProperty element = property.SerializedProperty.GetArrayElementAtIndex(i);

                        if (element.propertyType != SerializedPropertyType.ObjectReference)
                            continue;

                        Validate(element, property.ExpectedType);
                    }
                else
                    Validate(property.SerializedProperty, property.ExpectedType);
            }

            foreach (PropertyModel child in property.Children)
                ValidateProperty(child);
        }

        private static void Validate(
            SerializedProperty serializedProperty,
            Type expectedType)
        {
            Object value = serializedProperty.objectReferenceValue;

            if (value == null)
                return;

            if (value is GameObject gameObject
                && gameObject.TryGetComponent(expectedType, out Component component))
                value = component;

            if (value != null && !expectedType.IsAssignableFrom(value.GetType()))
            {
                Debug.LogError($"Saneject: '{value.GetType().Name}' does not implement {expectedType.Name}", value);
                value = null;
            }

            serializedProperty.objectReferenceValue = value;
        }

        #endregion

        #region Extensions

        /// <summary>
        /// Collects all drawable <see cref="FieldInfo" /> of the type, including base classes.
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldInfosForInspector(this Type type)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.DeclaredOnly;

            Stack<Type> chain = new();

            while (type != null && type != typeof(object))
            {
                chain.Push(type);
                type = type.BaseType;
            }

            foreach (FieldInfo field in chain.Select(t => t
                         .GetFields(flags)
                         .Where(f => f.Name != "m_Script")
                         .Where(f => !f.IsDefined(typeof(NonSerializedAttribute), false))
                         .Where(f => !f.IsDefined(typeof(HideInInspector), false))
                         .Where(f => f.IsPublic || f.IsDefined(typeof(SerializeField), false) || f.IsSerializeInterface())).SelectMany(validFields => validFields))
                yield return field;
        }

        /// <summary>
        /// Returns true if the field is marked with [ReadOnly].
        /// </summary>
        public static bool HasReadOnlyAttribute(this FieldInfo field)
        {
            return field.IsDefined(typeof(ReadOnlyAttribute), false);
        }

        /// <summary>
        /// Returns true if the field is marked with [Inject].
        /// </summary>
        public static bool HasInjectAttribute(this FieldInfo field)
        {
            return field.IsDefined(typeof(InjectAttribute), false);
        }

        /// <summary>
        /// Returns true if the field is marked with [SerializeInterface].
        /// </summary>
        public static bool IsSerializeInterface(this FieldInfo field)
        {
            return field.IsDefined(typeof(SerializeInterfaceAttribute), false);
        }

        #endregion
    }
}