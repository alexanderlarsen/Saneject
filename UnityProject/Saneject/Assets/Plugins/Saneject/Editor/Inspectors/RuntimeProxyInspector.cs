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
    /// Read-only custom inspector for <see cref="RuntimeProxyBase"/> assets.
    /// Displays runtime proxy configuration derived from bindings and the resolved Play Mode instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), CustomEditor(typeof(RuntimeProxyBase), true)]
    public class RuntimeProxyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            HelpBoxUtility.DrawHelpBox
            (
                "• Runtime proxies are serialized placeholder assets for interface references Unity cannot serialize directly across runtime boundaries.\n" +
                "• This inspector is read-only and reflects configuration created from FromRuntimeProxy() bindings.\n" +
                "• During scope startup, registered components swap these placeholders for resolved runtime instances.\n" +
                "• Accessing a proxy directly before swap throws InvalidOperationException.\n" +
                "• In Play Mode, Resolved Instance shows the concrete object this proxy resolved to."
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
                        tooltip: "Play Mode only. Shows the concrete object this runtime proxy resolved to. Null in Edit Mode and until resolution succeeds."
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
                HelpBoxUtility.DrawHelpBox
                (
                    "This creation-based runtime proxy will create an object that is destroyed on scene load because 'Dont Destroy On Load' is disabled.",
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
