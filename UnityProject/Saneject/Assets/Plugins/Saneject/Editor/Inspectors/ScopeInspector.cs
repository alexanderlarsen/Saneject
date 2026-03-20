using System.ComponentModel;
using System.Text;
using Plugins.Saneject.Editor.Data.Context;
using Plugins.Saneject.Editor.Inspectors.Models;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Inspectors
{
    [EditorBrowsable(EditorBrowsableState.Never), CustomEditor(typeof(Scope), true, isFallback = false), CanEditMultipleObjects]
    public class ScopeInspector : UnityEditor.Editor
    {
        private Scope scope;
        private ContextIdentity rootContextIdentity;
        private ContextIdentity contextIdentity;
        private ScopeHierarchyModel scopeHierarchyModel;
        private ComponentModel componentModel;

        private void OnEnable()
        {
            scope = (Scope)target;
            rootContextIdentity = new ContextIdentity(scope.transform.root);
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
                        "Select one scope to view its scope-specific inspector sections.\n" +
                        "To inject multiple scene hierarchies, use the injection menus or batch injection.",
                        MessageType.Info
                    );

                GUILayout.Label($"{targets.Length} scopes selected");
            }
            else
            {
                DrawHelpBox();
                DrawContext();
                DrawGlobalComponents();
                DrawProxySwapTargets();
                DrawScopeHierarchy();
            }

            DrawDefault();
        }

        private static void DrawHelpBox()
        {
            HelpBoxUtility.DrawHelpBox
            (
                "A Scope declares bindings for part of the hierarchy. During injection, Saneject resolves each injection site from the nearest Scope at the same transform or above, then falls back to parent Scopes when needed.\n\n" +
                "A context is a serialization boundary for GameObject hierarchies: scene object, prefab instance, or prefab asset. Context filtering decides which transforms enter a run. Context isolation decides whether resolution may cross scene-object and prefab-instance boundaries inside that run.\n\n" +
                "With context isolation enabled, only same-context Scopes and candidates are used. With it disabled, scene objects and prefab instances in the active hierarchy can resolve across those boundaries. If a dependency must cross a boundary Unity cannot serialize directly, bind it through a runtime proxy."
            );
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
            SerializedProperty property = serializedObject.FindProperty("globalComponents");

            bool isFoldedOut = EditorLayoutUtility.PersistentFoldout
            (
                text: $"Global Components ({property.arraySize})",
                tooltip: "Serialized components this scope will register in GlobalScope during Scope.Awake(). Runtime proxies using FromGlobalScope() can resolve them there, and runtime code can also query GlobalScope directly.",
                defaultFoldoutState: true,
                prefsKey: "Saneject.ScopeInspector.Foldouts.GlobalComponents"
            );

            if (!isFoldedOut)
                return;

            if (property.arraySize == 0)
            {
                EditorGUILayout.LabelField("No global components declared in this scope.");
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty element = property.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                }
            }
        }

        private void DrawProxySwapTargets()
        {
            SerializedProperty property = serializedObject.FindProperty("proxySwapTargets");

            bool isFoldedOut = EditorLayoutUtility.PersistentFoldout
            (
                text: $"Runtime Proxy Swap Targets ({property.arraySize})",
                tooltip: "Components in this scope whose single-value serialized interface members currently hold runtime proxy placeholders. During scope startup, Saneject asks them to swap those proxies for resolved runtime instances before normal Awake() methods run.",
                defaultFoldoutState: true,
                prefsKey: "Saneject.ScopeInspector.Foldouts.RuntimeProxySwapTargets"
            );

            if (!isFoldedOut)
                return;

            if (property.arraySize == 0)
            {
                EditorGUILayout.LabelField("No proxy swap targets declared in this scope.");
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty element = property.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                }
            }
        }

        private void DrawScopeHierarchy()
        {
            bool isFoldedOut = EditorLayoutUtility.PersistentFoldout
            (
                text: "Scope Hierarchy",
                tooltip: "Click a scope to navigate to its GameObject. When context isolation is enabled, scopes in different contexts are grayed out because they are outside the inspected scope's resolution boundary.",
                defaultFoldoutState: true,
                prefsKey: "Saneject.ScopeInspector.Foldouts.ScopeHierarchy"
            );

            if (!isFoldedOut)
                return;

            DrawHierarchyRecursive(scopeHierarchyModel);
            GUILayout.Space(2);
            return;

            static void DrawHierarchyRecursive(ScopeHierarchyModel model)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(EditorGUI.indentLevel * 15 + 2);
                    StringBuilder sb = new();

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

                    GUIStyle styleToCopy = model.IsCurrent
                        ? EditorStyles.boldLabel
                        : EditorStyles.label;

                    GUIStyle style = new(styleToCopy)
                    {
                        margin = EditorStyles.label.margin,
                        padding = EditorStyles.label.padding
                    };

                    Color textColor = style.normal.textColor;

                    textColor.a =
                        !ProjectSettings.UseContextIsolation || model.IsSameContext
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

        private void DrawDefault()
        {
            if (componentModel.Properties.Count == 0)
                return;

            EditorGUILayout.Space(4);
            componentModel.SerializedObject.Update();

            foreach (PropertyModel propertyData in componentModel.Properties)
            {
                SanejectInspector.DrawProperty(propertyData);
                SanejectInspector.ValidateProperty(propertyData);
            }

            componentModel.SerializedObject.ApplyModifiedProperties();
        }

        private void BuildScopeHierarchy()
        {
            Scope rootScope = scope.transform.root.GetComponentInChildren<Scope>();
            scopeHierarchyModel = new ScopeHierarchyModel(rootScope, scope, contextIdentity);
        }
    }
}
