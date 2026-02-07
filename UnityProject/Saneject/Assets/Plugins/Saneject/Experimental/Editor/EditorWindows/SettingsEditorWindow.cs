using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Utilities;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows
{
    public class SettingsEditorWindow : EditorWindow
    {
        private Vector2 userSettingsScrollPos;
        private Vector2 projectSettingsScrollPos;
        private GUIStyle paddingStyle;

        private void OnEnable()
        {
            paddingStyle = new GUIStyle
            {
                padding = new RectOffset(4, 4, 4, 4)
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(paddingStyle);

            DrawToolbar(out int selectedTabIndex);
            EditorGUILayout.Space(8);

            switch (selectedTabIndex)
            {
                case 0:
                {
                    DrawUserSettings();
                    break;
                }
                case 1:
                {
                    DrawProjectSettings();
                    break;
                }
            }

            EditorGUILayout.EndVertical();
        }

        #region Draw methods

        private void DrawToolbar(out int selectedTabIndex)
        {
            const string key = "Saneject.SettingsEditorWindow.SelectedTab";

            selectedTabIndex = GUILayout.Toolbar
            (
                selected: EditorPrefs.GetInt(key, defaultValue: 0),
                texts: new[]
                {
                    "User Settings",
                    "Project Settings"
                }
            );

            EditorPrefs.SetInt
            (
                key,
                selectedTabIndex
            );
        }

        private void DrawUserSettings()
        {
            GUILayout.Label("Personal editor settings stored locally for this user", EditorStyles.miniLabel);
            EditorGUILayout.Space(8);
            userSettingsScrollPos = EditorGUILayout.BeginScrollView(userSettingsScrollPos);
            EditorGUILayout.LabelField("Ask Before Injection", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Current Scene/Prefab",
                tooltip: "Show a confirmation dialog before injecting the entire current scene or prefab.",
                currentValue: UserSettings.AskBefore_Inject_CurrentScene_Or_CurrentPrefab,
                onChanged: newValue => UserSettings.AskBefore_Inject_CurrentScene_Or_CurrentPrefab = newValue
            );

            DrawToggle
            (
                label: "All Scene Objects/Scene Prefab Instances",
                tooltip: "Show a confirmation dialog before injecting all scene objects or prefab instances in the current scene.",
                currentValue: UserSettings.AskBefore_Inject_AllSceneObjects_Or_AllScenePrefabInstances,
                onChanged: newValue => UserSettings.AskBefore_Inject_AllSceneObjects_Or_AllScenePrefabInstances = newValue
            );

            DrawToggle
            (
                label: "Selected Scene Hierarchies",
                tooltip: "Show a confirmation dialog before injecting all selected scene hierarchies in the Scene Hierarchy window.",
                currentValue: UserSettings.AskBefore_Inject_SelectedSceneHierarchies,
                onChanged: newValue => UserSettings.AskBefore_Inject_SelectedSceneHierarchies = newValue
            );

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Ask Before Batch Injection", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Selected Assets",
                tooltip: "Show a confirmation dialog before batch injecting all selected assets or folders in the project window.",
                currentValue: UserSettings.AskBefore_BatchInject_SelectedAssets,
                onChanged: newValue => UserSettings.AskBefore_BatchInject_SelectedAssets = newValue
            );

            EditorGUILayout.Space(8);
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

            EditorGUILayout.Space(8);
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

            EditorGUILayout.Space(8);
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

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Use Default User Settings") &&
                DialogUtility.Settings.Confirm_UseDefaultUserSettings())
            {
                UserSettings.UseDefaultSettings();
                RepaintAllInspectors();
            }
        }

        private void DrawProjectSettings()
        {
            GUILayout.Label("Settings stored in the project and shared across all users", EditorStyles.miniLabel);
            EditorGUILayout.Space(8);
            projectSettingsScrollPos = EditorGUILayout.BeginScrollView(projectSettingsScrollPos);
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
                "and dependencies may freely resolve across scene and prefab instance boundaries.",
                currentValue: ProjectSettings.UseContextIsolation,
                onChanged: newValue => ProjectSettings.UseContextIsolation = newValue,
                repaintInspectors: true
            );

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Scope File Generation", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Generate Scope Namespace From Folder",
                tooltip: "When enabled, a new Scope created via 'Assets/Saneject/Create New Scope' gets a namespace matching its folder path (relative to Assets). When disabled, the Scope is created without a namespace.",
                currentValue: ProjectSettings.GenerateScopeNamespaceFromFolder,
                onChanged: newValue => ProjectSettings.GenerateScopeNamespaceFromFolder = newValue
            );

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Proxy Generation", EditorStyles.boldLabel);

            DrawToggle
            (
                label: "Generate Proxy Scripts On Domain Reload",
                tooltip: "When enabled, all proxy scripts are generated from your declared bindings on domain reload. This is usually the preferred option, but in some cases, this can slow domain reload down. If disabled, you can generate them manually from the Saneject menu.",
                currentValue: ProjectSettings.GenerateProxyScriptsOnDomainReload,
                onChanged: newValue => ProjectSettings.GenerateProxyScriptsOnDomainReload = newValue
            );

            DrawPathPicker
            (
                label: "Generated Proxy Asset Folder",
                tooltip: "Folder to save proxy assets auto-generated by the system.",
                currentPath: ProjectSettings.ProxyAssetGenerationFolder,
                onChanged: path => ProjectSettings.ProxyAssetGenerationFolder = path
            );

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Use Default Project Settings") &&
                DialogUtility.Settings.Confirm_UseDefaultProjectSettings())
            {
                ProjectSettings.UseDefaultSettings();
                RepaintAllInspectors();
            }
        }

        #endregion

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
                        string relativePath = "Assets" + selected[absProjectPath.Length..];

                        if (relativePath != currentPath)
                        {
                            onChanged(relativePath);
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