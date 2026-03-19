using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Plugins.Saneject.Runtime.Settings
{
    /// <summary>
    /// Provides access to user-specific settings stored locally for this user.
    /// <remarks>
    /// Settings are stored locally in <see cref="UnityEditor.EditorPrefs" />.
    /// Properties are only editable in the editor outside Play Mode and will return default values when accessed outside the editor.
    /// </remarks>
    /// </summary>
    public static class UserSettings
    {
        private const string SettingsPrefix = "SanejectSettings_";
 
        #region Ask Before Injection

        /// <summary>
        /// Show a confirmation dialog before injecting a whole scene.
        /// </summary>
        public static bool AskBeforeInjectScene
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        /// <summary>
        /// Show a confirmation dialog before injecting a prefab asset.
        /// </summary>
        public static bool AskBeforeInjectPrefabAsset
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        } 

        /// <summary>
        /// Show a confirmation dialog before injecting selected scene hierarchies.
        /// </summary>
        public static bool AskBeforeInjectSelectedSceneHierarchies
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        /// <summary>
        /// Show a confirmation dialog before batch injecting scenes and prefab assets.
        /// </summary>
        public static bool AskBeforeBatchInject
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Inspector

        /// <summary>
        /// Show [Inject] fields and [field: Inject] auto-properties in the Inspector.
        /// </summary>
        public static bool ShowInjectedFieldsProperties
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        /// <summary>
        /// Show help boxes in the Inspector.
        /// </summary>
        public static bool ShowHelpBoxes
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Play Mode Logging

        /// <summary>
        /// Log when a proxy instance is resolved at runtime.
        /// </summary>
        public static bool LogProxyResolve
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        /// <summary>
        /// Log when objects are registered or unregistered with the global scope at runtime.
        /// </summary>
        public static bool LogGlobalScopeRegistration
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Editor Logging

        /// <summary>
        /// Log a message when a binding is unused during injection.
        /// This can happen when a binding is registered in a scope but never used in the scene or prefab.
        /// </summary>
        public static bool LogUnusedBindings
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        /// <summary>
        /// Log a summary on injection complete.
        /// </summary>
        public static bool LogInjectionSummary
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        /// <summary>
        /// Clear console logs before injection starts. Useful for only seeing logs related to the latest injection run.
        /// </summary>
        public static bool ClearLogsOnInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        /// <summary>
        /// If enabled, Saneject will scan and report unused runtime proxy scripts and assets during domain reload.
        /// They are considered unused, if no Scope binding is referencing them. This can help you keep your project clean and organized.
        /// </summary>
        public static bool LogUnusedRuntimeProxiesOnDomainReload
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all user settings to their default values.
        /// </summary>
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
            return defaultValue;
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
            return defaultValue;
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