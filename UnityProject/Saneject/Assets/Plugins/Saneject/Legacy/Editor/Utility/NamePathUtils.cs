using System;
using System.Reflection;
using Plugins.Saneject.Legacy.Editor.Extensions;
using Plugins.Saneject.Legacy.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.Utility
{
    public static class NamePathUtils
    {
        /// <summary>
        /// Builds a slash-separated path to an injected field or property,
        /// combining GameObject hierarchy, component type, and property path.
        /// Example: "Root/MonoA/monoB".
        /// </summary>
        public static string GetInjectedFieldPath(
            SerializedObject serializedObject,
            SerializedProperty serializedProperty)
        {
            Component component = (Component)serializedObject.targetObject;
            string goPath = component.transform.GetHierarchyPath();
            string propertyPath = serializedProperty.propertyPath;

            FieldInfo field = serializedProperty.GetFieldInfo();
            bool isInterfaceBackingField = field.HasAttribute<InterfaceBackingFieldAttribute>();

            // Demangle compiler auto-property backing fields for ALL segments (handles nested objects).
            // Also trim Saneject's leading underscores on the LAST segment only when drawing interface-backing fields.
            {
                string[] parts = propertyPath.Split('.');

                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = DemangleCompilerBackingName(parts[i]);

                    // Only trim leading '_' on the final segment when this is an interface-backing field
                    if (i == parts.Length - 1 && isInterfaceBackingField)
                        parts[i] = parts[i].TrimStart('_');
                }

                propertyPath = string.Join(".", parts);
            }

            return $"{goPath}/{component.GetType().Name}/{propertyPath}";
        }

        /// <summary>
        /// Returns a display-friendly name for the given <see cref="SerializedProperty" />.
        /// If the property represents an auto-generated interface backing field,
        /// the leading underscore is removed so the name matches the original user-defined field.
        /// </summary>
        /// <param name="serializedProperty">
        /// The <see cref="SerializedProperty" /> to extract the display name from.
        /// </param>
        /// <returns>
        /// The display-ready property name, with leading underscores trimmed when backing
        /// fields are detected; otherwise the original property name.
        /// </returns>
        public static string GetDisplayName(this SerializedProperty serializedProperty)
        {
            string name = serializedProperty.name;
            FieldInfo field = serializedProperty.GetFieldInfo();
            bool isInterfaceBackingField = field.HasAttribute<InterfaceBackingFieldAttribute>();
            return isInterfaceBackingField ? name.TrimStart('_') : name;
        }

        /// <summary>
        /// Returns the logical name of a field, stripping compiler auto-property
        /// backing syntax (&lt;Name&gt;k__BackingField) when present.
        /// </summary>
        public static string GetLogicalName(FieldInfo field)
        {
            string n = field.Name;

            // Handle auto-property backing fields: <Name>k__BackingField
            if (n.Length > 0 && n[0] == '<')
            {
                int end = n.IndexOf(">k__BackingField", StringComparison.Ordinal);
                if (end > 1) return n[1..end]; // "InterfaceB1"
            }

            return n;
        }

        public static string GetComponentPath(this Component component)
        {
            string goPath = component.transform.GetHierarchyPath();
            return $"{goPath}/{component.GetType().Name}";
        }

        /// <summary>
        /// Returns the logical segment name: if the segment is a C# compiler
        /// auto-property backing field (&lt;Name&gt;k__BackingField) return "Name";
        /// otherwise return the segment unchanged. Safe for non-backing segments
        /// like "Array.data[0]" and normal field names.
        /// </summary>
        private static string DemangleCompilerBackingName(string segment)
        {
            if (segment.Length > 0 && segment[0] == '<')
            {
                int end = segment.IndexOf(">k__BackingField", StringComparison.Ordinal);

                if (end > 1)
                    return segment[1..end]; // "PropertyName"
            }

            return segment;
        }

        /// <summary>
        /// Returns the full GameObject hierarchy path from the scene root to this transform.
        /// Example: "Root/Child/Leaf".
        /// </summary>
        private static string GetHierarchyPath(this Transform transform)
        {
            return !transform.parent ? transform.name : $"{transform.parent.GetHierarchyPath()}/{transform.name}";
        }
    }
}