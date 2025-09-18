using System;
using System.Collections;
using System.Reflection;
using Plugins.Saneject.Editor.Utility;
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
            SanejectInspector.DrawMonoBehaviourScriptField(targets, target);

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

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Global Scene Objects", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                SceneGlobalContainer container = (SceneGlobalContainer)target;

                if (typeof(SceneGlobalContainer)
                        .GetField("globalBindings", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(container) is IList bindings)
                    foreach (object binding in bindings)
                    {
                        PropertyInfo typeProp = binding.GetType().GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
                        PropertyInfo instanceProp = binding.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Instance);

                        if (typeProp == null)
                            continue;

                        if (instanceProp == null)
                            continue;

                        Type type = (Type)typeProp.GetValue(binding);
                        Object instance = (Object)instanceProp.GetValue(binding);

                        string displayName = type != null ? ObjectNames.NicifyVariableName(type.Name) : "(Invalid Type)";
                        EditorGUILayout.ObjectField(displayName, instance, typeof(Object), true);
                    }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}