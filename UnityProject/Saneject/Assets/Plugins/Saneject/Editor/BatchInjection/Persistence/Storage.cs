using Plugins.Saneject.Editor.BatchInjection.Data;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.Persistence
{
    public static class Storage
    {
        private const string PrefsKey = "Saneject_BatchInject_Data";

        public static void SaveData(BatchInjectorData injectorData)
        {
            string json = JsonUtility.ToJson(injectorData);
            EditorPrefs.SetString(PrefsKey, json);
        }

        public static BatchInjectorData LoadOrCreateData()
        {
            if (!EditorPrefs.HasKey(PrefsKey))
                return new BatchInjectorData();

            string json = EditorPrefs.GetString(PrefsKey);
            return JsonUtility.FromJson<BatchInjectorData>(json) ?? new BatchInjectorData();
        }
    }
}