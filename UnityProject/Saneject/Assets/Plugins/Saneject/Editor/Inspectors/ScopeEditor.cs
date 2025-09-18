using System.Collections.Generic;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="Scope" /> components.
    /// Presents help, context, and runtime injection tools for both scene and prefab scopes.
    /// </summary>
    [CustomEditor(typeof(Scope), true), CanEditMultipleObjects]
    public class ScopeEditor : UnityEditor.Editor
    {
        private readonly List<(string text, Scope scope)> scopeHierarchy = new();

        private void OnEnable()
        {
            BuildScopeHierarchy();
            EditorApplication.hierarchyChanged += BuildScopeHierarchy;
            Debug.Log("OnEnable");
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= BuildScopeHierarchy;
            Debug.Log("OnDisable");
        }

        public override void OnInspectorGUI()
        {
            SanejectInspector.DrawMonoBehaviourScriptField(targets, target);
            
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

            if (UserSettings.ShowScopePath)
                for (int i = 0; i < scopeHierarchy.Count; i++)
                {
                    (string text, Scope scope) = scopeHierarchy[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(i == 0 ? "Scope Path" : " ",
                        GUILayout.Width(EditorGUIUtility.labelWidth));

                    EditorGUILayout.LabelField(text,
                        i == scopeHierarchy.Count - 1 ? EditorStyles.boldLabel : EditorStyles.label);

                    if (i != scopeHierarchy.Count - 1)
                    {
                        Texture icon = EditorGUIUtility.IconContent("d_GameObject Icon").image;

                        if (GUILayout.Button(new GUIContent("", $"Jump to {scope.gameObject.name} GameObject"), GUILayout.Width(18), GUILayout.Height(18)))
                        {
                            Selection.activeObject = scope.gameObject;
                            EditorGUIUtility.PingObject(scope.gameObject);
                        }

                        Rect r = GUILayoutUtility.GetLastRect();

                        if (Event.current.type == EventType.Repaint)
                        {
                            const float padTop = 3f;
                            const float padRight = 2f;
                            const float padBottom = 3f;
                            const float padLeft = 3f;

                            GUI.DrawTexture(
                                new Rect(
                                    r.x + padLeft,
                                    r.y + padTop,
                                    r.width - (padLeft + padRight),
                                    r.height - (padTop + padBottom)
                                ),
                                icon,
                                ScaleMode.ScaleToFit);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

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

        private void BuildScopeHierarchy()
        {
            if (!UserSettings.ShowScopePath)
                return;

            scopeHierarchy.Clear();

            Component current = target as Component;

            while (current != null)
            {
                if (current.TryGetComponent(out Scope scope))
                    scopeHierarchy.Add((
                        $"{scope.gameObject.name} ({scope.GetType().Name})",
                        scope
                    ));

                current = current.transform.parent;
            }

            scopeHierarchy.Reverse();
        }
    }
}