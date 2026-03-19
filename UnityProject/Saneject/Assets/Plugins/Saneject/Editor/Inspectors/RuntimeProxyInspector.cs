using System;
using System.ComponentModel;
using System.Reflection;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Proxy;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="RuntimeProxyBase" />.
    /// Displays configuration and runtime info for RuntimeProxy ScriptableObjects.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), CustomEditor(typeof(RuntimeProxyBase), true)]
    public class RuntimeProxyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            HelpBoxUtility.DrawHelpBox
            (
                "• Allows serialization of interface-based references across scenes and prefabs.\n" +
                "• Resolves or instantiates the actual object using the selected method at runtime.\n" +
                "• A source generator (ProxyObjectGenerator.dll) implements all interfaces and forwards calls to the real instance.\n" +
                "• Use the proxy like the real object - just through its interfaces.\n" +
                "• To make the proxy as inexpensive as possible, it only tries to resolve the instance once at runtime, the first time you access it."
            );

            EditorGUI.BeginDisabledGroup(true);

            serializedObject.Update();

            Type memberInfo = target.GetType().BaseType;

            if (memberInfo != null)
            {
                Object value = null;

                if (Application.isPlaying)
                {
                    FieldInfo field = memberInfo.GetField("resolvedInstance", BindingFlags.NonPublic | BindingFlags.Instance);
                    value = field?.GetValue(target) as Object;
                }

                EditorGUILayout.ObjectField
                (
                    new GUIContent
                    (
                        text: "Resolved Instance",
                        tooltip: "The instance that is resolved at runtime. Always null in edit mode."
                    ),
                    value,
                    objType: typeof(Object),
                    allowSceneObjects: true
                );
            }

            SerializedProperty resolveMethodProp = serializedObject.FindProperty("resolveMethod");
            SerializedProperty prefabProp = serializedObject.FindProperty("prefab");
            SerializedProperty dontDestroyProp = serializedObject.FindProperty("dontDestroyOnLoad");
            SerializedProperty instanceModeProp = serializedObject.FindProperty("instanceMode");

            EditorGUILayout.PropertyField(resolveMethodProp);

            string selected = resolveMethodProp.enumDisplayNames[resolveMethodProp.enumValueIndex];

            switch (selected)
            {
                case "From Component On Prefab":
                    EditorGUILayout.PropertyField(instanceModeProp);
                    EditorGUILayout.PropertyField(prefabProp);
                    EditorGUILayout.PropertyField(dontDestroyProp);
                    break;
                case "From New Component On New Game Object":
                    EditorGUILayout.PropertyField(instanceModeProp);
                    EditorGUILayout.PropertyField(dontDestroyProp);
                    break;
            }

            if (selected is "From Component On Prefab" or "From New Component On New Game Object" && !dontDestroyProp.boolValue)
                EditorGUILayout.HelpBox
                (
                    "This object will be destroyed on scene load. Unless you expect to re-resolve it, consider enabling 'Dont Destroy On Load'.",
                    MessageType.Warning
                );

            serializedObject.ApplyModifiedProperties();
            DrawInterfaceListAligned(target.GetType());
            EditorGUI.EndDisabledGroup();
        }

        private void DrawInterfaceListAligned(Type proxyType)
        {
            Type[] interfaces = proxyType.GetInterfaces();

            EditorGUILayout.BeginHorizontal();

            // Left column (label)
            GUILayout.Label("Interfaces", EditorStyles.wordWrappedLabel, GUILayout.Width(EditorGUIUtility.labelWidth));

            // Right column (list)
            if (interfaces.Length == 0)
            {
                EditorGUILayout.LabelField("No interfaces implemented.");
            }
            else
            {
                EditorGUILayout.BeginVertical();

                foreach (Type iface in interfaces)
                    GUILayout.Label(iface.Name, EditorStyles.label);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}