using System;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Settings
{
    /// <summary>
    /// Editor window for configuring Saneject <see cref="UserSettings" /> in the Unity Editor.
    /// Allows toggling editor and injection-related settings that affect Saneject's editor tooling and diagnostics.
    /// </summary>
    public class UserSettingsEditorWindow : EditorWindow
    {
        private Vector2 scrollPos;

        [MenuItem("Saneject/User Settings")]
        public static void ShowWindow()
        {
            UserSettingsEditorWindow editorWindow = GetWindow<UserSettingsEditorWindow>();
            editorWindow.titleContent = new GUIContent("Saneject User Settings");
            editorWindow.Show();
        }

        private static void DrawToggle(
            string label,
            string tooltip,
            bool currentValue,
            Action<bool> onChanged,
            bool repaintInspectors = false)
        {
            bool newValue = EditorGUILayout.ToggleLeft(new GUIContent(label, tooltip), currentValue);

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

            EditorGUILayout.TextField(
                string.IsNullOrEmpty(currentPath) ? "<none>" : currentPath,
                EditorStyles.textField,
                GUILayout.Height(EditorGUIUtility.singleLineHeight)
            );

            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absProjectPath = Application.dataPath; // full path to Assets/
                string selected = EditorUtility.OpenFolderPanel("Select Folder", absProjectPath, "");

                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected.StartsWith(absProjectPath))
                    {
                        string relPath = "Assets" + selected.Substring(absProjectPath.Length);

                        if (relPath != currentPath)
                        {
                            onChanged(relPath);
                            AssetDatabase.Refresh();

                            if (repaintInspectors)
                                RepaintAllInspectors();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Selected folder is not inside this project's Assets folder.");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void RepaintAllInspectors()
        {
            foreach (EditorWindow window in Resources.FindObjectsOfTypeAll<EditorWindow>())
                if (window.GetType().Name == "InspectorWindow")
                    window.Repaint();
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.LabelField("Injection", EditorStyles.boldLabel);

            DrawToggle(
                label: "Ask Before Scene Injection",
                tooltip: "Show a confirmation dialog before injecting all dependencies into the full scene (excluding prefabs present in the scene).",
                currentValue: UserSettings.AskBeforeSceneInjection,
                onChanged: newValue => UserSettings.AskBeforeSceneInjection = newValue
            );

            DrawToggle(
                label: "Ask Before Hierarchy Injection",
                tooltip: "Show a confirmation dialog before injecting dependencies into a single scene hierarchy (excluding prefabs present in the hierarchy).",
                currentValue: UserSettings.AskBeforeHierarchyInjection,
                onChanged: newValue => UserSettings.AskBeforeHierarchyInjection = newValue
            );

            DrawToggle(
                label: "Ask Before Prefab Injection",
                tooltip: "Show a confirmation dialog before injecting dependencies into a prefab.",
                currentValue: UserSettings.AskBeforePrefabInjection,
                onChanged: newValue => UserSettings.AskBeforePrefabInjection = newValue
            );

            DrawToggle(
                label: "Filter By Same Context",
                tooltip:
                "Filters out prefab components when injecting into scene objects, and filters out scene components when injecting into prefabs. This enforces context isolation. Disabling is not recommended: These links depend on a prefab being present in the scene and will break if the prefab is removed or reused somewhere else. Use ProxyObjects (BindComponent<IInterface, Concrete>().FromProxy()) for proper cross-context references. Only disable if you explicitly want scene ↔ prefab links that exist only in the current scene.",
                currentValue: UserSettings.FilterBySameContext,
                onChanged: newValue => UserSettings.FilterBySameContext = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            DrawToggle(
                label: "Show Injected Fields/Properties",
                tooltip: "Show [Inject] fields and [field: Inject] auto-properties in the Inspector.",
                currentValue: UserSettings.ShowInjectedFieldsProperties,
                onChanged: newValue => UserSettings.ShowInjectedFieldsProperties = newValue,
                repaintInspectors: true
            );

            DrawToggle(
                label: "Show Help Boxes",
                tooltip: "Show help boxes in the Inspector.",
                currentValue: UserSettings.ShowHelpBoxes,
                onChanged: newValue => UserSettings.ShowHelpBoxes = newValue,
                repaintInspectors: true
            );
            
            DrawToggle(
                label: "Show Scope Path",
                tooltip: "Display scope path, from ancestor scopes down to the selected scope, in the scope inspector.",
                currentValue: UserSettings.ShowScopePath,
                onChanged: newValue => UserSettings.ShowScopePath = newValue,
                repaintInspectors: true
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Play Mode Logging (Editor Only)", EditorStyles.boldLabel);

            DrawToggle(
                label: "Log On Proxy Instance Resolve",
                tooltip: "Log when a proxy instance is resolved at runtime.",
                currentValue: UserSettings.LogProxyResolve,
                onChanged: newValue => UserSettings.LogProxyResolve = newValue
            );

            DrawToggle(
                label: "Log Global Scope Register/Unregister",
                tooltip: "Log when objects are registered or unregistered with the global scope at runtime.",
                currentValue: UserSettings.LogGlobalScopeRegistration,
                onChanged: newValue => UserSettings.LogGlobalScopeRegistration = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Logging", EditorStyles.boldLabel);

            DrawToggle(
                label: "Log Injection Stats",
                tooltip: "Log stats on injection complete: Number of scopes processed, global dependencies added, injected fields, missing bindings, unused bindings, and injection duration.",
                currentValue: UserSettings.LogInjectionStats,
                onChanged: newValue => UserSettings.LogInjectionStats = newValue
            );

            DrawToggle(
                label: "Log Prefab Skipped During Scene Injection",
                tooltip: "Log when a prefab is skipped during scene injection.",
                currentValue: UserSettings.LogPrefabSkippedDuringSceneInjection,
                onChanged: newValue => UserSettings.LogPrefabSkippedDuringSceneInjection = newValue
            );

            DrawToggle(
                label: "Log Unused Bindings",
                tooltip: "Log a message when a binding is unused during injection. This can happen when a binding is registered in a scope but never used in the scene or prefab.",
                currentValue: UserSettings.LogUnusedBindings,
                onChanged: newValue => UserSettings.LogUnusedBindings = newValue
            );

            DrawToggle(
                label: "Clear Logs On Injection",
                tooltip: "Clear console logs before injection starts.",
                currentValue: UserSettings.ClearLogsOnInjection,
                onChanged: newValue => UserSettings.ClearLogsOnInjection = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Proxy Generation", EditorStyles.boldLabel);

            DrawPathPicker(
                label: "Generated Proxy Asset Folder",
                tooltip: "Folder to save proxy assets auto-generated by the system.",
                currentPath: UserSettings.ProxyAssetGenerationFolder,
                onChanged: path => UserSettings.ProxyAssetGenerationFolder = path);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Scope File Generation", EditorStyles.boldLabel);

            DrawToggle(
                label: "Generate Scope Namespace From Folder",
                tooltip: "When enabled, a new Scope created via 'Assets/Create/Saneject/Scope' gets a namespace matching its folder path (relative to Assets). When disabled, the Scope is created without a namespace.",
                currentValue: UserSettings.GenerateScopeNamespaceFromFolder,
                onChanged: newValue => UserSettings.GenerateScopeNamespaceFromFolder = newValue
            );

            EditorGUILayout.EndScrollView();
        }
    }
}