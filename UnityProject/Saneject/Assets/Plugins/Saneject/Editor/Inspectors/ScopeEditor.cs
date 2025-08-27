using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="Scope" /> components.
    /// Presents help, context, and runtime injection tools for both scene and prefab scopes.
    /// </summary>
    [CustomEditor(typeof(Scope), true), CanEditMultipleObjects]
    public class ScopeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (UserSettings.ShowHelpBoxes)
                EditorGUILayout.HelpBox(
                    "• Two scope types: Prefab and Scene (both managed by this Scope component).\n" +
                    "• The system auto-detects scope context (Prefab or Scene object).\n" +
                    "• Prefab scopes are skipped during scene injection and must resolve dependencies themselves.\n" +
                    "• Scopes define how dependencies are resolved in their downward hierarchy.\n" +
                    "• Lower (child) scopes override higher (parent) ones if identical bindings exist in different scopes.\n" +
                    "• If not resolved locally, the dependency resolver walks up through parent scopes and attempts to resolve from there.\n" +
                    "• Scope components are automatically stripped from builds using HideFlags.DontSaveInBuild.",
                    MessageType.None, true);

            serializedObject.Update();

            bool hasPrefab = false;
            bool hasScene = false;

            foreach (Object t in targets)
                if (t is Scope s)
                {
                    if (s.gameObject.IsPrefab())
                        hasPrefab = true;
                    else
                        hasScene = true;
                }

            bool mixed = hasPrefab && hasScene;

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            string label = mixed ? "Mixed" : hasPrefab ? "Prefab" : "Scene";
            EditorGUILayout.LabelField("Scope Type", label);

            SanejectInspector.DrawAllSerializedFields(serializedObject, target);

            if (Application.isPlaying)
                GUILayout.Label("Injection is editor-only. Exit Play Mode to inject.", EditorStyles.boldLabel);

            GUILayout.Space(3);

            if (mixed)
            {
                GUI.enabled = false;
                GUILayout.Button("Injection requires selecting only Prefab or only Scene scopes");
                GUI.enabled = true;
            }
            else if (hasPrefab)
            {
                if (GUILayout.Button("Inject Prefab Dependencies"))
                    foreach (Object t in targets)
                        if (t is Scope scope)
                            DependencyInjector.InjectPrefabDependencies(scope);
            }
            else if (hasScene)
            {
                if (GUILayout.Button("Inject Hierarchy Dependencies"))
                    foreach (Object t in targets)
                        if (t is Scope scope)
                            DependencyInjector.InjectSingleHierarchyDependencies(scope);
            }

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}