using System;
using System.Collections.Generic;
using System.Reflection;
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Utility
{
    public static class SanejectInspector
    {
        /// <summary>
        /// Draws the complete default Saneject MonoBehaviour inspector, including the script field, all serializable fields in declaration order, injection-aware read-only handling, custom UI and validation for [SerializeInterface] fields, and recursive drawing of nested serializable types.
        /// </summary>
        public static void DrawDefault(
            SerializedObject serializedObject,
            Object[] targets,
            Object target)
        {
            serializedObject.Update();
            DrawMonoBehaviourScriptField(targets, target);
            DrawAllSerializedFields(serializedObject, target);
            serializedObject.ApplyModifiedProperties();
        } 

        /// <summary>
        /// Draws the default script field at the top of a MonoBehaviour inspector.
        /// </summary>
        public static void DrawMonoBehaviourScriptField(
            Object[] targets,
            Object target)
        {
            if (targets.Length != 1 || target is not MonoBehaviour mono)
                return;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mono), typeof(MonoScript), false);
            }
        }

        /// <summary>
        /// Draws all serializable fields on the given target using custom logic for interface injection and nested types.
        /// </summary>
        public static void DrawAllSerializedFields(
            SerializedObject serializedObject,
            Object target)
        {
            foreach (FieldInfo field in GetOrderedFields(target.GetType()))
            {
                if (!ShouldDrawField(field))
                    continue;

                DrawSerializedField(field, serializedObject);
            }
        }

        /// <summary>
        /// Draws a single serialized field with support for interface backing, read-only handling, and nested types.
        /// </summary>
        public static void DrawSerializedField(
            FieldInfo field,
            SerializedObject serializedObject,
            SerializedProperty parent = null)
        {
            bool isReadOnly = IsReadOnly(field);

            using (new EditorGUI.DisabledScope(isReadOnly))
            {
                if (IsSerializeInterfaceField(field))
                {
                    if (!UserSettings.ShowInjectedFields) return;

                    if (!TryGetInterfaceBackingInfo(field, serializedObject, out SerializedProperty interfaceProperty, out Type interfaceType, parent)) return;

                    if (!interfaceType.IsInterface)
                    {
                        EditorGUILayout.HelpBox(
                            $"Unable to resolve interface type for {GetFormattedInterfaceLabel(field.Name, interfaceType)}",
                            MessageType.Error);

                        return;
                    }

                    if (IsCollection(interfaceProperty))
                    {
                        if (isReadOnly)
                            DrawReadOnlyCollection(interfaceProperty, interfaceType);
                        else
                            EditorGUILayout.PropertyField(interfaceProperty, new GUIContent(GetFormattedInterfaceLabel(field.Name, interfaceType)), true);

                        ValidateInterfaceCollection(interfaceProperty, interfaceType);
                    }
                    else
                    {
                        DrawInterfaceObjectField(interfaceProperty, interfaceType);
                    }

                    return;
                }

                SerializedProperty property = parent == null
                    ? serializedObject.FindProperty(field.Name)
                    : parent.FindPropertyRelative(field.Name);

                if (property == null)
                    return;

                if (IsCollection(property))
                {
                    if (isReadOnly)
                    {
                        DrawReadOnlyCollection(property);
                        return;
                    }

                    EditorGUILayout.PropertyField(property, true);
                    return;
                }

                if (IsNestedSerializable(field.FieldType))
                {
                    DrawNestedSerializable(property, field.FieldType, serializedObject);
                    return;
                }

                EditorGUILayout.PropertyField(property, true);
            }
        }

        /// <summary>
        /// Attempts to find the interface backing field and resolve its concrete element type.
        /// </summary>
        public static bool TryGetInterfaceBackingInfo(
            FieldInfo field,
            SerializedObject serializedObject,
            out SerializedProperty property,
            out Type interfaceType,
            SerializedProperty parent = null)
        {
            string backingName = "__" + field.Name;

            property = parent == null
                ? serializedObject.FindProperty(backingName)
                : parent.FindPropertyRelative(backingName);

            interfaceType = null;

            if (property == null) return false;

            Type fieldType = field.FieldType;
            interfaceType = GetElementType(fieldType);
            return interfaceType != null;
        }

        /// <summary>
        /// Returns true if the field should be drawn as read-only due to [Inject] or [ReadOnly].
        /// </summary>
        public static bool IsReadOnly(FieldInfo field)
        {
            return HasInject(field) || HasReadOnly(field);
        }

        /// <summary>
        /// Returns true if the field is marked with [Inject].
        /// </summary>
        public static bool HasInject(FieldInfo field)
        {
            return field.IsDefined(typeof(InjectAttribute), false);
        }

        /// <summary>
        /// Returns true if the field is marked with [ReadOnly].
        /// </summary>
        public static bool HasReadOnly(FieldInfo field)
        {
            return field.IsDefined(typeof(ReadOnlyAttribute), false);
        }

        /// <summary>
        /// Returns true if the given property is a collection (array or list).
        /// </summary>
        public static bool IsCollection(SerializedProperty prop)
        {
            return prop.isArray && prop.propertyType != SerializedPropertyType.String;
        }

        /// <summary>
        /// Returns true if the field is marked with [SerializeInterface].
        /// </summary>
        public static bool IsSerializeInterfaceField(FieldInfo field)
        {
            return field.IsDefined(typeof(SerializeInterfaceAttribute), false);
        }

        /// <summary>
        /// Draws a collection in read-only format, used for injected or readonly lists/arrays.
        /// </summary>
        public static void DrawReadOnlyCollection(
            SerializedProperty prop,
            Type interfaceType = null)
        {
            Rect fullRect = GUILayoutUtility.GetRect(
                1f,
                EditorGUIUtility.singleLineHeight,
                GUILayout.ExpandWidth(true));

            Rect bgRect = new(fullRect.x, fullRect.y, fullRect.width - 48, fullRect.height);

            if (bgRect.Contains(Event.current.mousePosition))
                EditorGUI.DrawRect(bgRect,
                    EditorGUIUtility.isProSkin
                        ? new Color(1f, 1f, 1f, 0.05f)
                        : new Color(0f, 0f, 0f, 0.1f));

            Rect foldoutRect = new(fullRect.x, fullRect.y, fullRect.width - 48, fullRect.height);
            prop.isExpanded = EditorGUI.Foldout(foldoutRect, prop.isExpanded, GUIContent.none, true);
            Rect labelRect = new(foldoutRect.x + 1, foldoutRect.y, foldoutRect.width - 1, foldoutRect.height);

            EditorGUI.LabelField(
                labelRect,
                interfaceType != null
                    ? GetFormattedInterfaceLabel(prop.name, interfaceType)
                    : ObjectNames.NicifyVariableName($"{prop.name}"),
                EditorStyles.boldLabel);

            Rect countRect = new(fullRect.xMax - 48, fullRect.y, 48, fullRect.height);
            EditorGUI.IntField(countRect, GUIContent.none, prop.arraySize);

            if (!prop.isExpanded)
                return;

            if (prop.arraySize == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(16f);
                GUILayout.Label("List is Empty");
                GUILayout.EndHorizontal();
            }
            else
            {
                for (int i = 0; i < prop.arraySize; i++)
                {
                    SerializedProperty element = prop.GetArrayElementAtIndex(i);
                    string label = $"Element {i}";

                    switch (element.propertyType)
                    {
                        case SerializedPropertyType.ObjectReference:
                            EditorGUILayout.ObjectField(label, element.objectReferenceValue, typeof(Object), false);
                            break;
                        case SerializedPropertyType.String:
                            EditorGUILayout.TextField(label, element.stringValue);
                            break;
                        case SerializedPropertyType.Integer:
                            EditorGUILayout.IntField(label, element.intValue);
                            break;
                        case SerializedPropertyType.Float:
                            EditorGUILayout.FloatField(label, element.floatValue);
                            break;
                        default:
                            EditorGUILayout.PropertyField(element, new GUIContent(label), true);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Draws an object field for an interface reference. Tries to resolve component from GameObject if GameObject is assigned.
        /// </summary>
        public static void DrawInterfaceObjectField(
            SerializedProperty property,
            Type interfaceType)
        {
            GUIContent label = new(GetFormattedInterfaceLabel(property.name, interfaceType));
            Object oldValue = property.objectReferenceValue;
            Object newValue = EditorGUILayout.ObjectField(label, oldValue, typeof(Object), true);

            if (newValue is GameObject go && go.TryGetComponent(interfaceType, out Component comp))
                newValue = comp;

            if (newValue != null && !interfaceType.IsAssignableFrom(newValue.GetType()))
                Debug.LogError($"Saneject: '{newValue.GetType().Name}' does not implement {interfaceType.Name}", newValue);

            // newValue = null;
            property.objectReferenceValue = newValue;
        }

        /// <summary>
        /// Formats a field name and interface type into a human-friendly label.
        /// </summary>
        public static string GetFormattedInterfaceLabel(
            string fieldName,
            Type interfaceType)
        {
            string name = fieldName.TrimStart('_');
            string label = ObjectNames.NicifyVariableName(name);
            return $"{label} ({interfaceType.Name})";
        }

        /// <summary>
        /// Returns all instance fields of a type and its base types (excluding MonoBehaviour), in declaration order.
        /// </summary>
        public static FieldInfo[] GetOrderedFields(Type type)
        {
            List<FieldInfo> fields = new();

            while (type != typeof(MonoBehaviour) && type != typeof(object) && type != null)
            {
                fields.InsertRange(
                    0,
                    type.GetFields(
                        BindingFlags.Instance |
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.DeclaredOnly));

                type = type.BaseType;
            }

            return fields.ToArray();
        }

        /// <summary>
        /// Returns true if the field is valid for drawing in the inspector.
        /// </summary>
        public static bool ShouldDrawField(FieldInfo field)
        {
            if (field.Name == "m_Script") return false;
            if (field.IsDefined(typeof(NonSerializedAttribute), false)) return false;
            if (field.IsDefined(typeof(HideInInspector), false)) return false;
            if (field.Name.StartsWith("<")) return false;
            if (field.IsDefined(typeof(InterfaceBackingFieldAttribute), false)) return false;
            if (HasInject(field) && !UserSettings.ShowInjectedFields) return false;

            return field.IsPublic
                   || field.IsDefined(typeof(SerializeField), false)
                   || IsSerializeInterfaceField(field);
        }

        /// <summary>
        /// Returns the element type of a collection type (array or List<>).
        /// </summary>
        public static Type GetElementType(Type fieldType)
        {
            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }

        /// <summary>
        /// Returns true if the type is a non-Unity serializable reference type suitable for custom nested drawing.
        /// </summary>
        public static bool IsNestedSerializable(Type type)
        {
            return type.IsClass
                   && type != typeof(string)
                   && !typeof(Object).IsAssignableFrom(type)
                   && (type.IsSerializable || type.IsDefined(typeof(SerializeField), true));
        }

        /// <summary>
        /// Draws the contents of a nested serializable object recursively using the same field rules.
        /// </summary>
        public static void DrawNestedSerializable(
            SerializedProperty property,
            Type type,
            SerializedObject serializedObject)
        {
            property.isExpanded = EditorGUILayout.Foldout(
                property.isExpanded,
                ObjectNames.NicifyVariableName(property.name),
                true);

            if (!property.isExpanded) return;

            EditorGUI.indentLevel++;

            foreach (FieldInfo subField in GetOrderedFields(type))
            {
                if (!ShouldDrawField(subField))
                    continue;

                DrawSerializedField(subField, serializedObject, property);
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Validates that objects in a collection implements an interface type. Tries to resolve component from GameObject if GameObject is assigned.
        /// </summary>
        public static void ValidateInterfaceCollection(
            SerializedProperty prop,
            Type interfaceType)
        {
            if (!prop.isArray || prop.propertyType == SerializedPropertyType.String) return;

            for (int i = 0; i < prop.arraySize; i++)
            {
                SerializedProperty property = prop.GetArrayElementAtIndex(i);

                if (property.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                Object newValue = property.objectReferenceValue;

                if (newValue is GameObject go && go.TryGetComponent(interfaceType, out Component comp))
                    newValue = comp;

                if (newValue != null && !interfaceType.IsAssignableFrom(newValue.GetType()))
                    Debug.LogError($"Saneject: '{newValue.GetType().Name}' does not implement {interfaceType.Name}", newValue);

                property.objectReferenceValue = newValue;
            }
        }
    }
}