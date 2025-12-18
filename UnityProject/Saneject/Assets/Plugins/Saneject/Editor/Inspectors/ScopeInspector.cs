using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Inspectors.API;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Extensions;
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
    public class ScopeInspector : UnityEditor.Editor
    {
        private readonly List<ScopeNode> scopeHierarchy = new();
        private Scope inspectedScope;

        private HashSet<Scope> rootScopes = new();

        private void OnEnable()
        {
            inspectedScope = target as Scope;
            rootScopes = targets.OfType<Scope>().Select(s => s.FindRootScope()).ToHashSet();
            BuildScopeHierarchy();
            EditorApplication.hierarchyChanged += BuildScopeHierarchy;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= BuildScopeHierarchy;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SelectionType selectionType = GetSelectionType();
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            SanejectInspector.DrawMonoBehaviourScriptField(target);
            DrawHelpBox();
            EditorGUILayout.LabelField("Scope Type", selectionType.ToString());
            DrawScopePath();
            SanejectInspector.CollectPropertyData(serializedObject, target, out IReadOnlyCollection<PropertyData> properties);
            SanejectInspector.DrawAndValidateProperties(properties);

            if (Application.isPlaying)
                GUILayout.Label("Injection is editor-only. Exit Play Mode to inject.", EditorStyles.boldLabel);

            GUILayout.Space(3);
            DrawButton(selectionType);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawButton(SelectionType selectionType)
        {
            switch (selectionType)
            {
                case SelectionType.Scene:
                {
                    string suffix = rootScopes.Count > 1 ? $" ({rootScopes.Count} hierarchies)" : string.Empty;

                    if (GUILayout.Button($"Inject Hierarchy Dependencies{suffix}"))
                        InjectHierarchy();

                    break;
                }

                case SelectionType.Prefab:
                {
                    string suffix = rootScopes.Count > 1 ? $" ({rootScopes.Count} prefabs)" : string.Empty;

                    if (GUILayout.Button($"Inject Prefab Dependencies{suffix}"))
                        InjectPrefab();

                    break;
                }

                case SelectionType.Mixed:
                {
                    GUI.enabled = false;
                    GUILayout.Button("Injection requires selecting Prefab or Scene scopes only");
                    GUI.enabled = true;
                    break;
                }
            }
        }

        private SelectionType GetSelectionType()
        {
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

            return hasPrefab && hasScene
                ? SelectionType.Mixed
                : hasPrefab
                    ? SelectionType.Prefab
                    : SelectionType.Scene;
        }

        private static void DrawHelpBox()
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
        }

        private void DrawScopePath()
        {
            if (!UserSettings.ShowScopePath)
                return;

            if (targets.Length > 1)
            {
                EditorGUILayout.LabelField("Path To Scope", "Select one scope to view its path");
                return;
            }

            for (int i = 0; i < scopeHierarchy.Count; i++)
            {
                ScopeNode node = scopeHierarchy[i];

                bool sameContext = !UserSettings.UseContextIsolation || node.AreSameContext;

                GUIStyle labelStyle = new(i == scopeHierarchy.Count - 1 ? EditorStyles.boldLabel : EditorStyles.label);
                Color textColor = labelStyle.normal.textColor;
                textColor.a = sameContext ? 1f : 0.5f;
                labelStyle.normal.textColor = textColor;

                GUIContent labelContent = new
                (
                    node.LabelText,
                    sameContext
                        ? string.Empty
                        : $"'{node.LabelText}' is in a different context than the selected Scope and will not participate in injection from this Scope.\n\nContext isolation can be toggled in Saneject → User Settings → Use Context Isolation, but it's recommended to keep it on."
                );

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i == 0 ? "Path To Scope" : " ", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.LabelField(labelContent, labelStyle);

                if (i != scopeHierarchy.Count - 1)
                {
                    Texture icon = EditorGUIUtility.IconContent("d_GameObject Icon").image;

                    if (GUILayout.Button(new GUIContent("", $"Jump to {node.GameObject.name} GameObject"), GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        Selection.activeObject = node.GameObject;
                        EditorGUIUtility.PingObject(node.GameObject);
                    }

                    Rect r = GUILayoutUtility.GetLastRect();

                    if (Event.current.type == EventType.Repaint)
                    {
                        const float padTop = 3f;
                        const float padRight = 2f;
                        const float padBottom = 3f;
                        const float padLeft = 3f;

                        GUI.DrawTexture
                        (
                            position: new Rect
                            (
                                r.x + padLeft,
                                r.y + padTop,
                                r.width - (padLeft + padRight),
                                r.height - (padTop + padBottom)
                            ),
                            image: icon,
                            scaleMode: ScaleMode.ScaleToFit
                        );
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void InjectHierarchy()
        {
            if (!Dialogs.Injection.ConfirmInjectHierarchy())
                return;

            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            foreach (Scope rootScope in rootScopes)
                DependencyInjector.InjectSingleHierarchy(rootScope);
        }

        private void InjectPrefab()
        {
            if (!Dialogs.Injection.ConfirmInjectPrefab())
                return;

            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            foreach (Scope rootScope in rootScopes)
                DependencyInjector.InjectPrefab(rootScope);
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
                    scopeHierarchy.Add(new ScopeNode(scope, inspectedScope));

                current = current.transform.parent;
            }

            scopeHierarchy.Reverse();
        }

        private class ScopeNode
        {
            public ScopeNode(
                Scope scope,
                Scope inspectedScope)
            {
                GameObject = scope.gameObject;
                LabelText = $"{scope.gameObject.name} ({scope.GetType().Name})";
                AreSameContext = ContextFilter.AreSameContext(scope, inspectedScope);
                IsPrefab = scope.gameObject.IsPrefab();
            }

            public GameObject GameObject { get; }
            public string LabelText { get; }
            public bool AreSameContext { get; }
            public bool IsPrefab { get; }
        }

        private enum SelectionType
        {
            Scene,
            Prefab,
            Mixed
        }
    }
}