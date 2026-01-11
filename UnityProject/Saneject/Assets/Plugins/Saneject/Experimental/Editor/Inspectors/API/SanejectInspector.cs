using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Collects PropertyData for all serializable fields of the given type and its base classes, including [SerializeInterface] fields.
        /// </summary>
        public static void CollectPropertyData(
            SerializedObject serializedObject,
            Object target,
            out IReadOnlyCollection<PropertyData> properties)
        {
            properties = CollectForType
            (
                root: serializedObject,
                parent: null,
                type: target.GetType()
            ).ToList();
        }

        /// <summary>
        /// Recursively collects PropertyData for all serializable fields of the given type and its base classes,
        /// including [SerializeInterface] fields and nested serializable types.
        /// </summary>
        private static IEnumerable<PropertyData> CollectForType(
            SerializedObject root,
            SerializedProperty parent,
            Type type)
        {
            foreach (FieldInfo field in type.GetAllFields())
            {
                if (!field.ShouldDraw())
                    continue;

                bool isSerializedInterface = field.IsSerializeInterface();
                Type elementType = field.ResolveElementType();
                string displayName = ObjectNames.NicifyVariableName(field.Name);

                if (isSerializedInterface)
                    displayName += " (" + elementType.Name + ")";

                string path = isSerializedInterface
                    ? field.GetInterfaceBackingFieldName()
                    : field.Name;

                SerializedProperty property = parent == null
                    ? root.FindProperty(path)
                    : parent.FindPropertyRelative(path);

                if (property == null)
                    continue;

                List<PropertyData> children = null;

                if (field.FieldType.IsNestedSerializable())
                    children = CollectForType(
                        root: root,
                        parent: property,
                        type: field.FieldType
                    ).ToList();

                yield return new PropertyData(
                    property,
                    displayName,
                    field.IsReadOnly(),
                    isSerializedInterface,
                    elementType,
                    children
                );
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the complete default Saneject MonoBehaviour inspector, including the script field, all serializable fields in declaration order, injection-aware read-only handling, custom UI and validation for [SerializeInterface] fields, and recursive drawing of nested serializable types.
        /// </summary>
        public static void DrawDefault(
            Object target,
            SerializedObject serializedObject)
        {
            DrawMonoBehaviourScriptField(target);
            CollectPropertyData(serializedObject, target, out IReadOnlyCollection<PropertyData> properties);
            DrawAndValidateProperties(properties);
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
        /// Draws a collection of <see cref="PropertyData" /> with the given display names, read-only flags and validates interface fields.
        /// </summary>
        public static void DrawAndValidateProperties(IEnumerable<PropertyData> properties)
        {
            foreach (PropertyData data in properties)
                DrawAndValidateProperty(data);
        }

        /// <summary>
        /// Draws a single <see cref="PropertyData" /> with the given display name, read-only flag and validates interface fields, including nested serializable types.
        /// </summary>
        /// <param name="data"></param>
        public static void DrawAndValidateProperty(PropertyData data)
        {
            if (data.Children is { Count: > 0 })
            {
                data.Property.isExpanded = EditorGUILayout.Foldout(
                    data.Property.isExpanded,
                    data.DisplayName,
                    true);

                if (data.Property.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    foreach (PropertyData child in data.Children)
                        DrawAndValidateProperty(child);

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(data.IsReadOnly))
                {
                    EditorGUILayout.PropertyField(data.Property, new GUIContent(data.DisplayName), true);
                }
            }

            if (data.IsSerializedInterface)
                ValidateProperty(data.Property, data.ExpectedType);
        }

        #endregion

        #region Helpers & extensions

        /// <summary>
        /// Validates that the property is assigned to an object that implements the expected type.
        /// </summary>
        public static void ValidateProperty(
            SerializedProperty property,
            Type expectedType)
        {
            // Validate collection
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

                    if (elementProperty.propertyType != SerializedPropertyType.ObjectReference)
                        continue;

                    Validate(elementProperty);
                }

                return;
            }

            // Validate single value
            Validate(property);
            return;

            void Validate(SerializedProperty prop)
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

        /// <summary>
        /// Returns true if the field should be drawn as read-only due to [Inject] or [ReadOnly].
        /// </summary>
        public static bool IsReadOnly(this FieldInfo field)
        {
            return field.HasInjectAttribute() || field.HasReadOnlyAttribute();
        }

        /// <summary>
        /// Returns true if the field is marked with [ReadOnly].
        /// </summary>
        public static bool HasReadOnlyAttribute(this FieldInfo field)
        {
            return field.IsDefined(typeof(ReadOnlyAttribute), false);
        }

        /// <summary>
        /// Returns true if the field is valid for drawing in the inspector.
        /// </summary>
        public static bool ShouldDraw(this FieldInfo field)
        {
            // TODO: Reorder these and review if NonSerializedAttribute and HideInInspector is necessary to specify now that I draw with default property field

            if (field.Name == "m_Script")
                return false;

            if (field.IsDefined(typeof(NonSerializedAttribute), false))
                return false;

            if (field.IsDefined(typeof(HideInInspector), false))
                return false;

            if (field.HasInjectAttribute() && !UserSettings.ShowInjectedFieldsProperties)
                return false;

            return field.IsPublic
                   || field.IsDefined(typeof(SerializeField), false)
                   || field.IsSerializeInterface();
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

        /// <summary>
        /// Returns the element type of a single or collection type (array/list).
        /// </summary>
        public static Type ResolveElementType(this FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;

            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }

        /// <summary>
        /// Returns the logical name of a field, stripping compiler auto-property
        /// backing syntax (&lt;Name&gt;k__BackingField) when present and adds two leading underscores.
        /// </summary>
        public static string GetInterfaceBackingFieldName(this FieldInfo field)
        {
            string name = field.Name;

            // Strip auto-property backing fields compiler names: <Name>k__BackingField
            if (name.Length > 0 && name[0] == '<')
            {
                int end = name.IndexOf(">k__BackingField", StringComparison.Ordinal);

                if (end > 1)
                    name = name[1..end]; // "InterfaceB1"
            }

            return $"__{name}";
        }

        /// <summary>
        /// Returns true if the type is a non-Unity serializable reference type suitable for custom nested drawing.
        /// </summary>
        public static bool IsNestedSerializable(this Type type)
        {
            return type.IsClass
                   && type != typeof(string)
                   && !typeof(Object).IsAssignableFrom(type)
                   && (type.IsSerializable || type.IsDefined(typeof(SerializeField), true));
        }

        /// <summary>
        /// Collects all <see cref="FieldInfo" /> of the type, including base classes.
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
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

                foreach (FieldInfo field in fields)
                    yield return field;
            }
        }

        #endregion
    }
}