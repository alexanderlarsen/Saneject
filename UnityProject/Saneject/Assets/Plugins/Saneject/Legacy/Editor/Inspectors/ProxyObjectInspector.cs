using System;
using System.Reflection;
using Plugins.Saneject.Legacy.Runtime.Proxy;
using Plugins.Saneject.Legacy.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Legacy.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="ProxyObjectBase" />.
    /// Displays configuration and runtime info for interface proxy ScriptableObjects.
    /// </summary>
    [CustomEditor(typeof(ProxyObjectBase), true)]
    public class ProxyObjectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (UserSettings.ShowHelpBoxes)
                EditorGUILayout.HelpBox(
                    "• Allows serialization of interface-based references across scenes and prefabs.\n" +
                    "• Resolves or instantiates the actual object using the selected method at runtime.\n" +
                    "• A source generator (ProxyObjectGenerator.dll) implements all interfaces and forwards calls to the real instance.\n" +
                    "• Use the proxy like the real object - just through its interfaces.\n" +
                    "• To make the proxy as inexpensive as possible, it only tries to resolve the instance once at runtime, the first time you access it.",
                    MessageType.None, true);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            serializedObject.Update();

            if (Application.isPlaying)
            {
                Type memberInfo = target.GetType().BaseType;

                if (memberInfo != null)
                {
                    FieldInfo field = memberInfo.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance);
                    Object value = field?.GetValue(target) as Object;
                    EditorGUILayout.ObjectField("Resolved Instance", value, typeof(Object), true);
                }
            }

            SerializedProperty resolveMethodProp = serializedObject.FindProperty("resolveMethod");
            SerializedProperty prefabProp = serializedObject.FindProperty("prefab");
            SerializedProperty dontDestroyProp = serializedObject.FindProperty("dontDestroyOnLoad");

            EditorGUILayout.PropertyField(resolveMethodProp);

            string selected = resolveMethodProp.enumDisplayNames[resolveMethodProp.enumValueIndex];

            switch (selected)
            {
                case "From Component On Prefab":
                    EditorGUILayout.PropertyField(prefabProp);
                    EditorGUILayout.PropertyField(dontDestroyProp);
                    break;
                case "From New Component On New Game Object":
                    EditorGUILayout.PropertyField(dontDestroyProp);
                    break;
            }

            if (selected is "From Component On Prefab" or "From New Component On New Game Object" &&
                !dontDestroyProp.boolValue)
                EditorGUILayout.HelpBox(
                    "This object will be destroyed on scene load. Unless you expect to re-resolve it, consider enabling 'Dont Destroy On Load'.",
                    MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();

            DrawInterfaceListAligned(target.GetType());
        }

        private void DrawWrappedLabel(string text)
        {
            GUILayout.Label(text, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawInterfaceListAligned(Type proxyType)
        {
            Type[] interfaces = proxyType.GetInterfaces();

            if (interfaces.Length == 0)
                return;

            EditorGUILayout.BeginHorizontal();

            // Left column (label)
            GUILayout.Label("Interfaces", EditorStyles.wordWrappedLabel, GUILayout.Width(EditorGUIUtility.labelWidth));

            // Right column (list)
            EditorGUILayout.BeginVertical();

            foreach (Type iface in interfaces)
                GUILayout.Label(iface.Name, EditorStyles.label);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}