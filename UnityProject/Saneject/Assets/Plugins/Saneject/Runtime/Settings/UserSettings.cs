using System.Runtime.CompilerServices;
using UnityEditor;

namespace Plugins.Saneject.Runtime.Settings
{
    /// <summary>
    /// Provides strongly-typed accessors for Saneject user/editor settings, backed by <see cref="UnityEditor.EditorPrefs" /> and is used by an <see cref="UnityEditor.EditorWindow" /> found under <c>Saneject/User Settings</c>.
    /// Settings affect injection prompts, inspector options, and logging behavior in the Unity Editor.
    /// These settings have no effect at runtime.
    /// </summary>
    public static class UserSettings
    {
        // Injection
        public static bool AskBeforeSceneInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }
        
        public static bool AskBeforePrefabInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        // Inspector
        public static bool ShowInjectedFields
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool ShowHelpBoxes
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        // Play Mode Logging
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

        // Editor Logging
        public static bool LogPrefabSkippedDuringSceneInjection
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool LogUnusedBindings
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
        }

        public static bool LogInjectionStats
        {
            get => GetBool(defaultValue: true);
            set => SetBool(value);
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
    }
}