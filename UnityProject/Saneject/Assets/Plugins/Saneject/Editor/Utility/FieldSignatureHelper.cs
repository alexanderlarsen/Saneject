using System.Text;
using Plugins.Saneject.Editor.Extensions;
using UnityEditor;

namespace Plugins.Saneject.Editor.Utility
{
    public static class FieldSignatureHelper
    {
        public static string GetInjectedFieldSignature(
            SerializedObject serializedObject,
            SerializedProperty serializedProperty,
            string injectId)
        {
            string fieldName = serializedProperty.GetFieldInfo().Name;

            StringBuilder sb = new();
            sb.Append("[Injected ");
            sb.Append(fieldName.Contains(">k__BackingField") ? "property" : "field");
            sb.Append(": ");
            sb.Append(NamePathUtils.GetInjectedFieldPath(serializedObject, serializedProperty));

            if (!string.IsNullOrWhiteSpace(injectId))
            {
                sb.Append(" | ID: ");
                sb.Append(injectId);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}