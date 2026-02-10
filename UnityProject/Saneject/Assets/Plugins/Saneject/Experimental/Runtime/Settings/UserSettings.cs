using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Experimental.Runtime.Settings
{
    /// <summary>
    /// Provides strongly-typed accessors for Saneject user/editor settings, backed by <see cref="UnityEditor.EditorPrefs" /> and is used by an <see cref="UnityEditor.EditorWindow" /> found under <c>Saneject/User Settings</c>.
    /// Settings affect injection prompts, inspector options, and logging behavior in the Unity Editor.
    /// These settings have no effect at runtime.
    /// </summary>
    public static class UserSettings
    {
        private const string SettingsPrefix = "SanejectSettings_";

        #region Ask Before Batch Injection

        public static bool AskBefore_BatchInject_SelectedAssets
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Ask Before Injection

        public static bool AskBefore_Inject_CurrentScene_Or_CurrentPrefab
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool AskBefore_Inject_AllSceneObjects_Or_AllScenePrefabInstances
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool AskBefore_Inject_SelectedSceneHierarchies
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Inspector

        public static bool ShowInjectedFieldsProperties
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool ShowHelpBoxes
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Play Mode Logging

        public static bool LogProxyResolve
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool LogGlobalScopeRegistration
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Editor Logging

        public static bool LogUnusedBindings
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool LogInjectionSummary
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool ClearLogsOnInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        public static bool LogUnusedRuntimeProxiesOnDomainReload
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Methods

        public static void UseDefaultSettings()
        {
#if UNITY_EDITOR
            string[] prefsKeys = typeof(UserSettings)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(prop => prop.CanWrite)
                .Select(prop => $"{SettingsPrefix}{prop.Name}")
                .Where(EditorPrefs.HasKey)
                .ToArray();

            foreach (string key in prefsKeys)
                EditorPrefs.DeleteKey(key);

            Debug.Log("Saneject: All user settings were reset to default values.");
#endif
        }

        #endregion

        #region Shared helpers

        private static string GetString(
            string defaultValue,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            string key = $"{SettingsPrefix}{propertyName}";
            return !EditorPrefs.HasKey(key) ? defaultValue : EditorPrefs.GetString(key);
#else
            return string.Empty;
#endif
        }

        private static void SetString(
            string value,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning("Saneject: User settings can only be modified in edit mode.");
                return;
            }

            string key = $"{SettingsPrefix}{propertyName}";
            EditorPrefs.SetString(key, value);
#endif
        }

        private static bool GetBool(
            bool defaultValue,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            string key = $"{SettingsPrefix}{propertyName}";
            return !EditorPrefs.HasKey(key) ? defaultValue : EditorPrefs.GetBool(key);
#else
            return false;
#endif
        }

        private static void SetBool(
            bool value,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning("Saneject: User settings can only be modified in edit mode.");
                return;
            }

            string key = $"{SettingsPrefix}{propertyName}";
            EditorPrefs.SetBool(key, value);
#endif
        }

        #endregion
    }
}