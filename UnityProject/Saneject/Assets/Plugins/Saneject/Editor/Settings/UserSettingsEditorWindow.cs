using System;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.Settings
{
    /// <summary>
    /// Editor window for configuring Saneject <see cref="UserSettings"/> in the Unity Editor.
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

        private static void DrawToggleWithRepaint(
            string label,
            bool currentValue,
            Action<bool> onChanged)
        {
            bool newValue = EditorGUILayout.ToggleLeft(label, currentValue);

            if (newValue != currentValue)
            {
                onChanged(newValue);
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

            UserSettings.AskBeforeSceneInjection = EditorGUILayout.ToggleLeft("Ask Before Scene Injection", UserSettings.AskBeforeSceneInjection);
            UserSettings.AskBeforePrefabInjection = EditorGUILayout.ToggleLeft("Ask Before Prefab Injection", UserSettings.AskBeforePrefabInjection);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            DrawToggleWithRepaint(
                "Show Injected Fields",
                UserSettings.ShowInjectedFields,
                newValue => UserSettings.ShowInjectedFields = newValue
            );

            DrawToggleWithRepaint(
                "Show Help Boxes",
                UserSettings.ShowHelpBoxes,
                newValue => UserSettings.ShowHelpBoxes = newValue
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Play Mode Logging (Editor Only)", EditorStyles.boldLabel);

            UserSettings.LogProxyResolve = EditorGUILayout.ToggleLeft("Log On Proxy Instance Resolve", UserSettings.LogProxyResolve);
            UserSettings.LogGlobalScopeRegistration = EditorGUILayout.ToggleLeft("Log Global Scope Register/Unregister", UserSettings.LogGlobalScopeRegistration);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Logging", EditorStyles.boldLabel);

            UserSettings.LogInjectionStats = EditorGUILayout.ToggleLeft("Log Injection Stats", UserSettings.LogInjectionStats);
            UserSettings.LogPrefabSkippedDuringSceneInjection = EditorGUILayout.ToggleLeft("Log Prefab Skipped During Scene Injection", UserSettings.LogPrefabSkippedDuringSceneInjection);
            UserSettings.LogUnusedBindings = EditorGUILayout.ToggleLeft("Log Unused Bindings", UserSettings.LogUnusedBindings);

            EditorGUILayout.EndScrollView();
        }
    }
}