using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows
{
    public class SettingsEditorWindow : EditorWindow
    {
        private Vector2 scrollPos;

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.LabelField("Ask Before Injection", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Current Scene/Prefab",
                tooltip: "Show a confirmation dialog before injecting the entire current scene or prefab.",
                currentValue: UserSettings.AskBeforeCurrentScenePrefabInjection,
                onChanged: newValue => UserSettings.AskBeforeCurrentScenePrefabInjection = newValue
            );

            DrawToggle
            (
                label: "All Scene Objects/Prefab Instances",
                tooltip: "Show a confirmation dialog before injecting all scene objects or prefab instances in the current scene.",
                currentValue: UserSettings.AskBeforeAllSceneObjectsPrefabInstancesInjection,
                onChanged: newValue => UserSettings.AskBeforeAllSceneObjectsPrefabInstancesInjection = newValue
            );

            DrawToggle
            (
                label: "Selected Scene Hierarchies",
                tooltip: "Show a confirmation dialog before injecting all selected scene hierarchies in the Scene Hierarchy window.",
                currentValue: UserSettings.AskBeforeSelectedSceneHierarchiesInjection,
                onChanged: newValue => UserSettings.AskBeforeSelectedSceneHierarchiesInjection = newValue
            );

            DrawToggle
            (
                label: "Selected Assets (Batch Injection)",
                tooltip: "Show a confirmation dialog before batch injection all selected assets or folders in the project window.",
                currentValue: UserSettings.AskBeforeSelectedAssetsBatchInjection,
                onChanged: newValue => UserSettings.AskBeforeSelectedAssetsBatchInjection = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Context Isolation", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Use Context Isolation",
                tooltip:
                "Controls who is allowed to see what.\n\n" +
                "When enabled, scenes and prefab instances are treated as separate contexts with a hard boundary. " +
                "Scene injection ignores prefab instances entirely, and prefab injection ignores scene objects. " +
                "Dependencies can only resolve within the same context.\n\n" +
                "When disabled, scene- and prefab instances can bleed into each other, fetching dependencies from each other. " +
                "Prefab assets are always context isolated, regardless of this setting." +
                "and dependencies may freely resolve across scene and prefab instance boundaries.\n\n" +
                "NOTE: Keeping this enabled is recommended for most use cases to preserve isolation and reuse safety.",
                currentValue: UserSettings.UseContextIsolation,
                onChanged: newValue => UserSettings.UseContextIsolation = newValue,
                repaintInspectors: true
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Show Injected Fields/Properties",
                tooltip: "Show [Inject] fields and [field: Inject] auto-properties in the Inspector.",
                currentValue: UserSettings.ShowInjectedFieldsProperties,
                onChanged: newValue => UserSettings.ShowInjectedFieldsProperties = newValue,
                repaintInspectors: true
            );

            DrawToggle
            (
                label: "Show Help Boxes",
                tooltip: "Show help boxes in the Inspector.",
                currentValue: UserSettings.ShowHelpBoxes,
                onChanged: newValue => UserSettings.ShowHelpBoxes = newValue,
                repaintInspectors: true
            );

            DrawToggle
            (
                label: "Show Scope Path",
                tooltip: "Display scope path, from ancestor scopes down to the selected scope, in the scope inspector.",
                currentValue: UserSettings.ShowScopePath,
                onChanged: newValue => UserSettings.ShowScopePath = newValue,
                repaintInspectors: true
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Play Mode Logging (Editor Only)", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Log On Proxy Instance Resolve",
                tooltip: "Log when a proxy instance is resolved at runtime.",
                currentValue: UserSettings.LogProxyResolve,
                onChanged: newValue => UserSettings.LogProxyResolve = newValue
            );

            DrawToggle
            (
                label: "Log Global Scope Register/Unregister",
                tooltip: "Log when objects are registered or unregistered with the global scope at runtime.",
                currentValue: UserSettings.LogGlobalScopeRegistration,
                onChanged: newValue => UserSettings.LogGlobalScopeRegistration = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Logging", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Log Injection Summary",
                tooltip: "Log summary on injection complete: Number of scopes processed, globals registered, injected fields/properties, missing dependencies/bindings, invalid/unused bindings, suppressed error count, and injection run duration.",
                currentValue: UserSettings.LogInjectionSummary,
                onChanged: newValue => UserSettings.LogInjectionSummary = newValue
            );

            DrawToggle
            (
                label: "Log Unused Bindings",
                tooltip: "Log a message when a binding is unused during injection. This can happen when a binding is registered in a scope but never used in the scene or prefab.",
                currentValue: UserSettings.LogUnusedBindings,
                onChanged: newValue => UserSettings.LogUnusedBindings = newValue
            );

            DrawToggle
            (
                label: "Clear Logs On Injection",
                tooltip: "Clear console logs before injection starts.",
                currentValue: UserSettings.ClearLogsOnInjection,
                onChanged: newValue => UserSettings.ClearLogsOnInjection = newValue
            );
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Scope File Generation", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Generate Scope Namespace From Folder",
                tooltip: "When enabled, a new Scope created via 'Assets/Create/Saneject/Scope' gets a namespace matching its folder path (relative to Assets). When disabled, the Scope is created without a namespace.",
                currentValue: UserSettings.GenerateScopeNamespaceFromFolder,
                onChanged: newValue => UserSettings.GenerateScopeNamespaceFromFolder = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Proxy Generation", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Generate Proxy Scripts On Domain Reload",
                tooltip: "When enabled, all proxy scripts are generated from your declared bindings on domain reload. This is usually the preferred option, but in some cases, this can slow domain reload down. If disabled, you can generate them manually from the Saneject menu.",
                currentValue: UserSettings.GenerateProxyScriptsOnDomainReload,
                onChanged: newValue => UserSettings.GenerateProxyScriptsOnDomainReload = newValue
            );

            DrawPathPicker(
                label: "Generated Proxy Asset Folder",
                tooltip: "Folder to save proxy assets auto-generated by the system.",
                currentPath: UserSettings.ProxyAssetGenerationFolder,
                onChanged: path => UserSettings.ProxyAssetGenerationFolder = path);

            EditorGUILayout.EndScrollView();
        }

        #region Utility methods

        private static void DrawToggle(
            string label,
            string tooltip,
            bool currentValue,
            Action<bool> onChanged,
            bool repaintInspectors = false)
        {
            bool newValue = EditorGUILayout.ToggleLeft
            (
                label: new GUIContent(label, tooltip),
                value: currentValue
            );

            if (newValue != currentValue)
            {
                onChanged(newValue);

                if (repaintInspectors)
                    RepaintAllInspectors();
            }
        }

        private static void DrawPathPicker(
            string label,
            string tooltip,
            string currentPath,
            Action<string> onChanged,
            bool repaintInspectors = false)
        {
            EditorGUILayout.LabelField(new GUIContent(label, tooltip));
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.TextField
            (
                text: string.IsNullOrEmpty(currentPath) ? "<none>" : currentPath,
                style: EditorStyles.textField,
                options: GUILayout.Height(EditorGUIUtility.singleLineHeight)
            );

            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absProjectPath = Application.dataPath; // full path to Assets/
                string selected = EditorUtility.OpenFolderPanel("Select Folder", absProjectPath, "");

                if (!string.IsNullOrEmpty(selected))
                {
                    if (!selected.StartsWith(absProjectPath))
                    {
                        Debug.LogWarning("Saneject: Selected folder is not inside this project's Assets folder.");
                    }
                    else
                    {
                        string relPath = "Assets" + selected[absProjectPath.Length..];

                        if (relPath != currentPath)
                        {
                            onChanged(relPath);
                            AssetDatabase.Refresh();

                            if (repaintInspectors)
                                RepaintAllInspectors();
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void RepaintAllInspectors()
        {
            IEnumerable<EditorWindow> inspectorWindows = Resources
                .FindObjectsOfTypeAll<EditorWindow>()
                .Where(window => window.GetType().Name == "InspectorWindow");

            foreach (EditorWindow window in inspectorWindows)
                window.Repaint();
        }

        #endregion
    }
}