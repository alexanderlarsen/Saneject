using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Runtime.Settings
{
    public static class ProjectSettings
    {
        private static readonly string Folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/Saneject"));
        private static readonly string FullPath = Path.Combine(Folder, "ProjectSettings.json");

        private static JObject cachedModel;

        #region Context Isolation

        public static bool UseContextIsolation
        {
            get => Get(defaultValue: false);
            set => Set(value);
        }

        #endregion

        #region Scope File Generation

        public static bool GenerateScopeNamespaceFromFolder
        {
            get => Get(defaultValue: true);
            set => Set(value);
        }

        #endregion

        #region Proxy Generation

        public static bool GenerateProxyScriptsOnDomainReload
        {
            get => Get(defaultValue: true);
            set => Set(value);
        }

        public static string ProxyAssetGenerationFolder
        {
            get => Get(defaultValue: "Assets/SanejectGenerated/RuntimeProxies");
            set => Set(value);
        }

        #endregion

        #region Methods

        public static void UseDefaultSettings()
        {
#if UNITY_EDITOR
            cachedModel = new JObject();
            SaveModel(cachedModel);
            Debug.LogWarning("Saneject: All project settings were reset to default values. This affects all users of this project.");
#endif
        }

        #endregion

        #region Shared helpers

        private static T Get<T>(
            T defaultValue,
            [CallerMemberName] string propertyName = null)
        {
#if UNITY_EDITOR
            if (propertyName == null)
                return defaultValue;

            JObject model = LoadModel();

            if (!model.TryGetValue(propertyName, out JToken token))
                return defaultValue;

            try
            {
                return token.ToObject<T>();
            }
            catch (Exception e)
            {
                Debug.LogWarning(
                    $"Saneject: Failed to read project setting '{propertyName}'. Using default. ({e.Message})");

                return defaultValue;
            }
#else
            return defaultValue;
#endif
        }

        private static void Set(
            object value,
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

            JObject model = LoadModel();
            model[propertyName] = JToken.FromObject(value);
            SaveModel(model);
#endif
        }

        private static JObject LoadModel()
        {
#if UNITY_EDITOR
            if (cachedModel != null)
                return cachedModel;

            if (!File.Exists(FullPath))
                return cachedModel = new JObject();

            try
            {
                string json = File.ReadAllText(FullPath);
                return cachedModel = JObject.Parse(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Saneject: Failed to load project settings. {e.Message}. Resetting to defaults.");
                cachedModel = new JObject();
                SaveModel(cachedModel);
                return cachedModel;
            }
#else
            return null;
#endif
        }

        private static void SaveModel(JObject model)
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(Folder);
            string json = model.ToString(Formatting.Indented);
            File.WriteAllText(FullPath, json);
            cachedModel = model;
#endif
        }

        #endregion
    }
}