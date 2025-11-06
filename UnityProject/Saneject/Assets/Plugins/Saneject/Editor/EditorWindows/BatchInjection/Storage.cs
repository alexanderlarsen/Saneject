using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    public static class Storage
    {
        private const string PrefsKey = "Saneject_BatchInject_Data";

        public static void SaveData(BatchInjectorData data)
        {
            string json = JsonUtility.ToJson(data);
            EditorPrefs.SetString(PrefsKey, json);
        }

        public static BatchInjectorData LoadData()
        {
            if (!EditorPrefs.HasKey(PrefsKey))
                return new BatchInjectorData();

            string json = EditorPrefs.GetString(PrefsKey);
            BatchInjectorData data = JsonUtility.FromJson<BatchInjectorData>(json) ?? new BatchInjectorData();
            data.Initialize();
            return data;
        }
    }
}