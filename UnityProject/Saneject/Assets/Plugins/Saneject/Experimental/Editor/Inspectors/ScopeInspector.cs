using System.Text;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Inspectors.Models;
using Plugins.Saneject.Experimental.Editor.Utilities;
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
                DrawHelpBox();
                DrawContext();
                DrawScopeHierarchy();
                DrawGlobalComponents();
                DrawInjectButtons();
            }

            DrawDefault();
        }

        private static void DrawHelpBox()
        {
            if (!UserSettings.ShowHelpBoxes)
                return;

            EditorGUILayout.HelpBox(
                "A Scope is where you declare bindings. When a component needs a dependency, it is resolved from the nearest Scope on the same level or above it in the hierarchy, with fallback to parent Scopes if no local binding is found.\n\n" +
                "Bindings are only used for objects that belong to the same context as the Scope. A context is a serialization boundary: Scene Object, Prefab Instance, Prefab Asset, or Global (assets like ScriptableObjects, textures, audio clips, etc.).\n\n" +
                "With context isolation enabled, scopes do not cross contexts. Scene objects and prefab instances are resolved separately, even if they live in the same hierarchy. Scopes from other contexts are shown in the hierarchy but grayed out because they will not be used.\n\n" +
                "Unity already prevents prefab assets from referencing scene objects, but prefab instances can reference scene objects and vice versa. Saneject blocks this by default because injection happens at edit time. Allowing it would make the prefab depend on the current scene and break when moved to another scene.\n\n" +
                "If you need cross context references, use proxies. Disabling context isolation is an escape hatch for debugging, not a recommended architectural pattern.",
                MessageType.None,
                true
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
                text: "Global Components",
                tooltip: "This scope declares global components. These are automatically added to GlobalScope on Scope.Awake, removed on Scope.OnDestroy, and can be fetched from the GlobalScope by proxies or manually.",
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

        private void DrawScopeHierarchy()
        {
            bool isFoldedOut = EditorLayoutUtility.PersistentFoldout
            (
                text: "Scope Hierarchy",
                tooltip: "Click on a hierarchy item to navigate to its GameObject. Different context scopes are grayed out.",
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
                        !UserSettings.UseContextIsolation || model.IsSameContext
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
            bool isFoldedOut = EditorLayoutUtility.PersistentFoldout
            (
                text: "Injection Controls",
                tooltip: string.Empty,
                defaultFoldoutState: true,
                prefsKey: "Saneject.ScopeInspector.Foldouts.InjectionControls"
            );

            if (!isFoldedOut)
                return;

            switch (contextIdentity.Type)
            {
                case ContextType.SceneObject or ContextType.PrefabInstance:
                {
                    EditorGUILayout.LabelField("Inject Entire Scene By Context");

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(new GUIContent(
                                text: "All",
                                tooltip: "Injects the entire scene, including prefabs instances.")))
                            InjectionUtility.Inject.CurrentScene();

                        if (GUILayout.Button(new GUIContent(
                                text: "Scene Objects",
                                tooltip: "Injects all scene objects, excluding prefabs instances.")))
                            InjectionUtility.Inject.AllSceneObjects();

                        if (GUILayout.Button(new GUIContent(
                                text: "Prefab Instances",
                                tooltip: "Injects all prefab instances in the scene.")))
                            InjectionUtility.Inject.AllScenePrefabInstances();
                    }

                    EditorGUILayout.LabelField("Inject Hierarchy By Context");

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(new GUIContent(
                                text: "All",
                                tooltip: "Injects the entire hierarchy, including both scene objects and prefabs instances.")))
                            InjectionUtility.Inject.Single_SceneHierarchy_ByContext(scope.gameObject, ContextWalkFilter.All);

                        if (GUILayout.Button(new GUIContent(
                                text: "This",
                                tooltip: "Injects all hierarchy objects that are the same context as this scope.")))
                            InjectionUtility.Inject.Single_SceneHierarchy_ByContext(scope.gameObject, ContextWalkFilter.SameAsStartObjects);

                        if (GUILayout.Button(new GUIContent(
                                text: "Scene Objects",
                                tooltip: "Injects all scene objects in the hierarchy, excluding prefabs instances.")))
                            InjectionUtility.Inject.Single_SceneHierarchy_ByContext(scope.gameObject, ContextWalkFilter.SceneObjects);

                        if (GUILayout.Button(new GUIContent(
                                text: "Prefab Instances",
                                tooltip: "Injects all prefab instances in the hierarchy.")))
                            InjectionUtility.Inject.Single_SceneHierarchy_ByContext(scope.gameObject, ContextWalkFilter.PrefabInstances);
                    }

                    break;
                }

                case ContextType.PrefabAsset:
                {
                    if (GUILayout.Button(new GUIContent(
                            text: "Inject Prefab",
                            tooltip: "Injects the entire prefab asset.")))
                        InjectionUtility.Inject.SpecificPrefabAsset(scope.gameObject);

                    break;
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