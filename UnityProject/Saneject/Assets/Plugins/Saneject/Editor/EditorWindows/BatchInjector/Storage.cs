using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    public static class Storage
    {
        private const string PrefsKey = "Saneject_BatchInject_Data";

        public static void SaveData(BatchInjectData data)
        {
            string json = JsonUtility.ToJson(data);
            EditorPrefs.SetString(PrefsKey, json);
        }

        public static BatchInjectData LoadData()
        {
            if (!EditorPrefs.HasKey(PrefsKey))
                return new BatchInjectData();

            string json = EditorPrefs.GetString(PrefsKey);
            return JsonUtility.FromJson<BatchInjectData>(json) ?? new BatchInjectData();
        }
    }
}