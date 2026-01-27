using System.Runtime.CompilerServices;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Runtime.Settings
{
    /// <summary>
    /// Provides strongly-typed accessors for Saneject user/editor settings, backed by <see cref="UnityEditor.EditorPrefs" /> and is used by an <see cref="UnityEditor.EditorWindow" /> found under <c>Saneject/User Settings</c>.
    /// Settings affect injection prompts, inspector options, and logging behavior in the Unity Editor.
    /// These settings have no effect at runtime.
    /// </summary>
    public static class UserSettings
    {
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

        public static bool AskBeforeSelectedAssetsBatchInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        #endregion
        
        #region Context Isolation
 
        public static bool UseContextIsolation
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

        public static bool ShowScopePath
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

        #endregion
        
        #region Scope File Generation

        public static bool GenerateScopeNamespaceFromFolder
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        #endregion
        
        #region Proxy Generation

        public static bool GenerateProxyScriptsOnDomainReload
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        public static string ProxyAssetGenerationFolder
        {
            get => GetString(defaultValue: "Assets/Generated");
            set => SetString(value);
        }

        #endregion
 
        #region Shared helpers

        private static string GetString(
            string defaultValue,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            string key = $"UserSettings_{propertyName}";
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
            string key = $"UserSettings_{propertyName}";
            EditorPrefs.SetString(key, value);
#endif
        }

        private static bool GetBool(
            bool defaultValue,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            string key = $"UserSettings_{propertyName}";
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
            string key = $"UserSettings_{propertyName}";
            EditorPrefs.SetBool(key, value);
#endif
        }

        #endregion
    }
}