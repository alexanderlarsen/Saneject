using System.Text;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Inspectors.Models;
using Plugins.Saneject.Experimental.Runtime.Scopes;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Inspectors
{
    [CustomEditor(typeof(Scope), true, isFallback = false), CanEditMultipleObjects]
    public class ScopeInspector : UnityEditor.Editor
    {
        private Scope scope;
        private ContextIdentity contextIdentity;
        private ScopeHierarchyModel scopeHierarchyModel;
        private ComponentModel componentModel;

        private void OnEnable()
        {
            scope = target as Scope;
            contextIdentity = new ContextIdentity(scope);
            componentModel = new ComponentModel(target, serializedObject);
            BuildScopeHierarchy();

            EditorApplication.hierarchyChanged += BuildScopeHierarchy;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= BuildScopeHierarchy;
        }

        public override void OnInspectorGUI()
        {
            SanejectInspector.DrawMonoBehaviourScriptField(target);

            if (targets.Length > 1)
            {
                if (UserSettings.ShowHelpBoxes)
                    EditorGUILayout.HelpBox
                    (
                        "Select one scope to view its inspector.\n" +
                        "If you want to inject multiple hierarchies in one click, use the injection menus.",
                        MessageType.Info
                    );

                GUILayout.Label($"{targets.Length} scopes selected");
            }
            else
            {
                // DrawHelpBox();
                DrawContext();
                DrawGlobalComponents();
                DrawScopeHierarchy();
                DrawInjectButtons();
            }

            if (componentModel.Properties.Count == 0)
                return;

            EditorGUILayout.Space(8);
            componentModel.SerializedObject.Update();

            foreach (PropertyModel propertyData in componentModel.Properties)
            {
                SanejectInspector.DrawProperty(propertyData);
                SanejectInspector.ValidateProperty(propertyData);
            }

            componentModel.SerializedObject.ApplyModifiedProperties();
        }

        private static void DrawHelpBox()
        {
            if (!UserSettings.ShowHelpBoxes)
                return;

            EditorGUILayout.HelpBox("N/A", MessageType.None, true);
        }

        private void DrawContext()
        {
            EditorGUILayout.LabelField
            (
                "Context",
                contextIdentity.ToString()
            );
        }

        private void DrawGlobalComponents()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                SerializedProperty globalComponentsProp = serializedObject.FindProperty("globalComponents");

                if (globalComponentsProp.arraySize > 1)
                    EditorGUILayout.PropertyField(globalComponentsProp);
            }
        }

        private void DrawScopeHierarchy()
        {
            const string foldOutKey = "Saneject.Scope.ScopeHierarchy.Foldout";
            bool isFoldedOut = EditorPrefs.GetBool(foldOutKey, true);

            isFoldedOut = EditorGUILayout.Foldout
            (
                foldout: isFoldedOut,
                content: new GUIContent(
                    "Scope Hierarchy",
                    "Click on a hierarchy item to navigate to its GameObject. Different context scopes are grayed out."
                ),
                toggleOnLabelClick: true
            );

            EditorPrefs.SetBool(foldOutKey, isFoldedOut);

            if (!isFoldedOut)
                return;

            DrawHierarchyRecursive(scopeHierarchyModel);
            EditorGUILayout.Space(2);

            return;

            static void DrawHierarchyRecursive(ScopeHierarchyModel model)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(EditorGUI.indentLevel * 15f);
                    StringBuilder sb = new();

                    if (model.IsCurrent)
                        sb.Append("→ ");

                    sb.Append($"{model.GameObject.name} ({model.ScopeName})");

                    GUIContent labelContent = new
                    (
                        text: sb.ToString(),
                        tooltip: $"GameObject: {model.GameObject.name}\n" +
                                 $"Scope: {model.ScopeName}\n" +
                                 $"Context Identity: {model.ContextIdentity}\n\n" +
                                 (model.IsCurrent
                                     ? "You are currently inspecting this Scope."
                                     : $"Click to navigate to GameObject: {model.GameObject.name}")
                    );

                    GUIStyle style = new
                    (
                        model.IsCurrent
                            ? EditorStyles.miniBoldLabel
                            : EditorStyles.miniLabel
                    )
                    {
                        margin = EditorStyles.miniLabel.margin,
                        padding = EditorStyles.miniLabel.padding
                    };

                    Color textColor = style.normal.textColor;

                    textColor.a = !UserSettings.UseContextIsolation || model.IsSameContext
                        ? 1
                        : 0.5f;

                    style.normal.textColor = textColor;
                    style.hover.textColor = textColor;
                    style.onHover.textColor = textColor;
                    style.focused.textColor = textColor;
                    style.active.textColor = textColor;

                    if (model.IsCurrent)
                    {
                        GUILayout.Label(labelContent, style);
                    }
                    else if (GUILayout.Button(labelContent, style))
                    {
                        Selection.activeObject = model.GameObject;
                        EditorGUIUtility.PingObject(model.GameObject);
                    }
                }

                foreach (ScopeHierarchyModel child in model.Children)
                {
                    EditorGUI.indentLevel++;
                    DrawHierarchyRecursive(child);
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawInjectButtons()
        {
            if (!UserSettings.ShowInjectButtonsInScopeInspector)
                return;

            switch (contextIdentity.Type)
            {
                case ContextType.SceneObject or ContextType.PrefabInstance:
                {
                    if (GUILayout.Button("Inject Scene"))
                        Debug.Log("Inject Scene");

                    if (GUILayout.Button("Inject Hierarchy (All)"))
                        Debug.Log("Inject Hierarchy (All)");

                    if (GUILayout.Button("Inject Hierarchy (Scene Objects)"))
                        Debug.Log("Inject Hierarchy (Scene Objects)");

                    if (GUILayout.Button("Inject Hierarchy (Prefab Instances)"))
                        Debug.Log("Inject Hierarchy (Prefab Instances)");

                    break;
                }

                case ContextType.PrefabAsset:
                {
                    if (GUILayout.Button("Inject Prefab"))
                        Debug.Log("Inject Prefab");

                    break;
                }
            }
        }

        private void BuildScopeHierarchy()
        {
            Scope rootScope = scope.transform.root.GetComponentInChildren<Scope>();
            scopeHierarchyModel = new ScopeHierarchyModel(rootScope, scope, contextIdentity);
        }
    }
}