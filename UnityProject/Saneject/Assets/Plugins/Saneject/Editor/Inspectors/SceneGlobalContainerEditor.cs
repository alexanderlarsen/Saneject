using System;
using Plugins.Saneject.Runtime.Global;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="SceneGlobalContainer" />.
    /// Displays registered global objects and enforces Saneject's rule that only one container exists per scene.
    /// Global containers are created and managed automatically; manual editing is discouraged.
    /// </summary>
    [CustomEditor(typeof(SceneGlobalContainer))]
    public class SceneGlobalContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (UserSettings.ShowHelpBoxes)
                EditorGUILayout.HelpBox(
                    $"• {nameof(SceneGlobalContainer)} is managed automatically by Saneject.\n" +
                    $"• It stores serialized objects registered with {nameof(Scope)}.RegisterGlobalComponent or {nameof(Scope)}.RegisterGlobalObject during injection and registers them with the static {nameof(GlobalScope)} (a service locator).\n" +
                    "• Manual container creation is not allowed. Extra instances are removed to ensure only one container exists per scene.\n" +
                    "• If no global bindings are no longer present, the container is deleted upon injection since it's not needed.",
                    MessageType.None, true);

            serializedObject.Update();

            SerializedProperty bindingsProp = serializedObject.FindProperty("globalBindings");

            if (bindingsProp.arraySize == 0)
                EditorGUILayout.LabelField("[No global objects currently registered in this scene]", EditorStyles.wordWrappedLabel);

            EditorGUILayout.LabelField(
                "References",
                EditorStyles.boldLabel);

            for (int i = 0; i < bindingsProp.arraySize; i++)
            {
                SerializedProperty bindingProp = bindingsProp.GetArrayElementAtIndex(i);
                SerializedProperty typeNameProp = bindingProp.FindPropertyRelative("typeName");
                SerializedProperty instanceProp = bindingProp.FindPropertyRelative("instance");

                string typeName = "(Invalid Type)";

                if (!string.IsNullOrEmpty(typeNameProp.stringValue))
                {
                    Type type = Type.GetType(typeNameProp.stringValue);

                    if (type != null)
                        typeName = type.Name;
                }

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(typeName, instanceProp.objectReferenceValue, typeof(Object), true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}