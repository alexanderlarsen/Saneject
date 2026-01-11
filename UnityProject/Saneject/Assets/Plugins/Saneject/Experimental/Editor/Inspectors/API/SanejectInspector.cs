using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Attributes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Experimental.Editor.Inspectors.API
{
    public static class SanejectInspector
    {
        #region Data collection

        /// <summary>
        /// Recursively collects InspectableField for all serializable fields of the given type and its base classes,
        /// including [SerializeInterface] fields and nested serializable types.
        /// </summary>
        public static IEnumerable<FieldData> GetFields(
            SerializedObject root,
            Type type,
            SerializedProperty parent = null)
        {
            foreach (FieldInfo field in type.GetFieldInfos())
            {
                bool isSerializedInterface = field.IsSerializeInterface();
                Type elementType = field.FieldType.ResolveElementType();
                string displayName = ObjectNames.NicifyVariableName(field.Name);
                string path = field.Name;

                if (isSerializedInterface)
                {
                    displayName += " (" + elementType.Name + ")";
                    path = $"__{BackingFieldNameUtility.GetLogicalName(field.Name)}";
                }

                SerializedProperty property = parent == null
                    ? root.FindProperty(path)
                    : parent.FindPropertyRelative(path);

                if (property == null)
                    continue;

                yield return new FieldData
                (
                    property: property,
                    displayName: displayName,
                    isReadOnly: field.HasInjectAttribute() || field.HasReadOnlyAttribute(),
                    isSerializedInterface: isSerializedInterface,
                    expectedType: elementType,
                    children: field.FieldType.IsNestedSerializable()
                        ? GetFields(root, field.FieldType, property).ToList()
                        : new List<FieldData>()
                );
            }
        }

        /// <summary>
        /// Collects all drawable <see cref="FieldInfo" /> of the type, including base classes.
        /// </summary>
        private static IEnumerable<FieldInfo> GetFieldInfos(this Type type)
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

            foreach (Type t in chain)
            {
                FieldInfo[] fields = t.GetFields(flags);

                foreach (FieldInfo field in fields.Where(field => field.IsValid()))
                    yield return field;
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the complete default Saneject MonoBehaviour inspector, including the script field, all serializable fields in declaration order, injection-aware read-only handling, custom UI and validation for [SerializeInterface] fields, and recursive drawing of nested serializable types.
        /// </summary>
        public static void OnInspectorGUI(
            Object target,
            SerializedObject serializedObject)
        {
            serializedObject.Update();

            DrawMonoBehaviourScriptField(target);

            foreach (FieldData field in GetFields(serializedObject, target.GetType()))
            {
                DrawField(field);

                if (field.IsSerializedInterface)
                    ValidateAssignment(field);
            }

            serializedObject.ApplyModifiedProperties();
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
        /// Draws a single <see cref="FieldData" /> with the given display name, read-only flag and validates interface fields, including nested serializable types.
        /// </summary>
        /// <param name="data"></param>
        public static void DrawField(FieldData data)
        {
            if (data.Children.Count <= 0)
            {
                using (new EditorGUI.DisabledScope(data.IsReadOnly))
                {
                    EditorGUILayout.PropertyField
                    (
                        data.Property,
                        new GUIContent(data.DisplayName),
                        true
                    );
                }

                return;
            }

            data.Property.isExpanded = EditorGUILayout.Foldout
            (
                data.Property.isExpanded,
                new GUIContent(data.DisplayName),
                true,
                EditorStyles.foldoutHeader
            );

            if (!data.Property.isExpanded)
                return;

            EditorGUI.indentLevel++;

            foreach (FieldData child in data.Children)
                DrawField(child);

            EditorGUI.indentLevel--;
        }

        #endregion
        
        #region Validation

        /// <summary>
        /// Validates that the field is assigned to an object that implements the expected type.
        /// </summary>
        public static void ValidateAssignment(FieldData field)
        {
            // Validate collection
            if (field.Property.isArray && field.Property.propertyType != SerializedPropertyType.String)
            {
                for (int i = 0; i < field.Property.arraySize; i++)
                {
                    SerializedProperty elementProperty = field.Property.GetArrayElementAtIndex(i);

                    if (elementProperty.propertyType != SerializedPropertyType.ObjectReference)
                        continue;

                    Validate(elementProperty, field.ExpectedType);
                }

                return;
            }

            // Validate single value
            Validate(field.Property, field.ExpectedType);
            return;

            static void Validate(
                SerializedProperty prop,
                Type expectedType)
            {
                Object value = prop.objectReferenceValue;

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

                prop.objectReferenceValue = value;
            }
        }

        #endregion

        #region Extensions

        /// <summary>
        /// Returns true if the field is valid for drawing in the inspector.
        /// </summary>
        public static bool IsValid(this FieldInfo field)
        {
            if (field.Name == "m_Script")
                return false;

            if (field.IsDefined(typeof(NonSerializedAttribute), false))
                return false;

            if (field.IsDefined(typeof(HideInInspector), false))
                return false;

            if (field.HasInjectAttribute() && !UserSettings.ShowInjectedFieldsProperties)
                return false;

            return field.IsPublic ||
                   field.IsDefined(typeof(SerializeField), false) ||
                   field.IsSerializeInterface();
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