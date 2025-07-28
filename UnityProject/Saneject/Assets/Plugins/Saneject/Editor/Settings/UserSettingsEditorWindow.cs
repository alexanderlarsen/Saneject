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

        private static void DrawToggleWithTooltip(
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

            DrawToggleWithTooltip(
                label: "Ask Before Scene Injection",
                tooltip: "Show a confirmation dialog before injecting dependencies into the scene.",
                currentValue: UserSettings.AskBeforeSceneInjection,
                onChanged: newValue => UserSettings.AskBeforeSceneInjection = newValue
            );

            DrawToggleWithTooltip(
                label: "Ask Before Prefab Injection",
                tooltip: "Show a confirmation dialog before injecting prefab dependencies.",
                currentValue: UserSettings.AskBeforePrefabInjection,
                onChanged: newValue => UserSettings.AskBeforePrefabInjection = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            DrawToggleWithTooltip(
                label: "Show Injected Fields",
                tooltip: "Show [Inject] fields in the Inspector.",
                currentValue: UserSettings.ShowInjectedFields,
                onChanged: newValue => UserSettings.ShowInjectedFields = newValue,
                repaintInspectors: true
            );

            DrawToggleWithTooltip(
                label: "Show Help Boxes",
                tooltip: "Show help boxes in the Inspector.",
                currentValue: UserSettings.ShowHelpBoxes,
                onChanged: newValue => UserSettings.ShowHelpBoxes = newValue,
                repaintInspectors: true
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Play Mode Logging (Editor Only)", EditorStyles.boldLabel);

            DrawToggleWithTooltip(
                label: "Log On Proxy Instance Resolve",
                tooltip: "Log when a proxy instance is resolved at runtime.",
                currentValue: UserSettings.LogProxyResolve,
                onChanged: newValue => UserSettings.LogProxyResolve = newValue
            );

            DrawToggleWithTooltip(
                label: "Log Global Scope Register/Unregister",
                tooltip: "Log when objects are registered or unregistered with the global scope at runtime.",
                currentValue: UserSettings.LogGlobalScopeRegistration,
                onChanged: newValue => UserSettings.LogGlobalScopeRegistration = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Logging", EditorStyles.boldLabel);

            DrawToggleWithTooltip(
                label: "Log Injection Stats",
                tooltip: "Log stats on injection complete: Number of scopes processed, global dependencies added, injected fields, missing bindings, unused bindings, and injection duration.",
                currentValue: UserSettings.LogInjectionStats,
                onChanged: newValue => UserSettings.LogInjectionStats = newValue
            );

            DrawToggleWithTooltip(
                label: "Log Prefab Skipped During Scene Injection",
                tooltip: "Log when a prefab is skipped during scene injection.",
                currentValue: UserSettings.LogPrefabSkippedDuringSceneInjection,
                onChanged: newValue => UserSettings.LogPrefabSkippedDuringSceneInjection = newValue
            );

            DrawToggleWithTooltip(
                label: "Log Unused Bindings",
                tooltip: "Log a message when a binding is unused during scene injection. This can happen when a binding is registered in a scope but never used in the scene.",
                currentValue: UserSettings.LogUnusedBindings,
                onChanged: newValue => UserSettings.LogUnusedBindings = newValue
            );

            EditorGUILayout.EndScrollView();
        }
    }
}