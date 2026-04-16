using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Plugins.Saneject.Runtime.Settings
{
    /// <summary>
    /// Provides access to project-scoped Saneject settings stored in <c>ProjectSettings/Saneject/ProjectSettings.json</c>.
    /// </summary>
    /// <remarks>
    /// These settings are shared with the Unity project. They can be changed only in Edit Mode and fall back to default values outside the Unity Editor.
    /// </remarks>
    public static class ProjectSettings
    {
        private static readonly string Folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/Saneject"));
        private static readonly string FullPath = Path.Combine(Folder, "ProjectSettings.json");

        #region Context Isolation

        /// <summary>
        /// Enables strict context boundaries during resolution.
        /// When enabled, scene and prefab-instance dependencies resolve only inside the same context.
        /// Prefab assets are always context-isolated, regardless of this setting.
        /// </summary>
        public static bool UseContextIsolation
        {
            get => Get(model => model.useContextIsolation);
            set => Set((model, newValue) => model.useContextIsolation = newValue, value);
        }

        #endregion

        #region Scope File Generation

        /// <summary>
        /// When enabled, a new Scope created via 'Assets/Saneject/Create New Scope' gets a namespace matching its folder path.
        /// When disabled, the Scope is created without a namespace.
        /// </summary>
        public static bool GenerateScopeNamespaceFromFolder
        {
            get => Get(model => model.generateScopeNamespaceFromFolder);
            set => Set((model, newValue) => model.generateScopeNamespaceFromFolder = newValue, value);
        }

        #endregion

        #region Proxy Generation

        /// <summary>
        /// When enabled, all proxy scripts are generated from declared bindings on domain reload.
        /// When disabled, they must be generated manually from the Saneject menu.
        /// </summary>
        public static bool GenerateProxyScriptsOnDomainReload
        {
            get => Get(model => model.generateProxyScriptsOnDomainReload);
            set => Set((model, newValue) => model.generateProxyScriptsOnDomainReload = newValue, value);
        }

        /// <summary>
        /// Target folder for auto-generated runtime proxy scripts and assets.
        /// </summary>
        public static string ProxyAssetGenerationFolder
        {
            get => Get(model => model.proxyAssetGenerationFolder);
            set => Set((model, newValue) => model.proxyAssetGenerationFolder = newValue, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all project settings to their default values.
        /// </summary>
        public static void UseDefaultSettings()
        {
#if UNITY_EDITOR
            SaveModel(new ProjectSettingsModel());
            Debug.LogWarning("Saneject: All project settings were reset to default values. This affects all users of this project.");
#endif
        }

        #endregion

        #region Shared helpers

        private static T Get<T>(
            Func<ProjectSettingsModel, T> selector,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            if (propertyName == null)
                return default;

            try
            {
                return selector(LoadModel());
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    $"Saneject: Failed to read project setting '{propertyName}'. Using default. ({e.Message})");

                return selector(new ProjectSettingsModel());
            }
#else
            return default;
#endif
        }

        private static void Set<T>(
            Action<ProjectSettingsModel, T> setter,
            T value,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.LogWarning("Saneject: Project settings can only be modified in edit mode.");
                return;
            }

            if (propertyName == null)
                return;

            ProjectSettingsModel model = LoadModel();
            setter(model, value);
            SaveModel(model);
#endif
        }

        private static ProjectSettingsModel LoadModel()
        {
#if UNITY_EDITOR
            ProjectSettingsModel model = new();

            if (!File.Exists(FullPath))
                return model;

            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(FullPath), model);
                return model;
            }
            catch (Exception e)
            {
                Debug.LogError($"Saneject: Failed to load project settings. {e.Message}. Resetting to defaults.");
                SaveModel(model);
                return model;
            }
#else
            return null;
#endif
        }

        private static void SaveModel(ProjectSettingsModel model)
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(Folder);
            string json = JsonUtility.ToJson(model, prettyPrint: true);
            File.WriteAllText(FullPath, json);
#endif
        }

        [Serializable]
        private sealed class ProjectSettingsModel
        {
            public bool useContextIsolation;
            public bool generateScopeNamespaceFromFolder = true;
            public bool generateProxyScriptsOnDomainReload = true;
            public string proxyAssetGenerationFolder = "Assets/SanejectGenerated/RuntimeProxies";
        }

        #endregion
    }
}
