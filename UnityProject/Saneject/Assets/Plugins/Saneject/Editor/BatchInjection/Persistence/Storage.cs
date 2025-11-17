using System;
using System.IO;
using Plugins.Saneject.Editor.BatchInjection.Data;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection.Persistence
{
    public static class Storage
    {
        private static readonly string Folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/Saneject"));
        private static readonly string FullPath = Path.Combine(Folder, "BatchInjectorData.json");

        public static void SaveData(BatchInjectorData injectorData)
        {
            Directory.CreateDirectory(Folder);
            string json = JsonUtility.ToJson(injectorData, prettyPrint: true);
            File.WriteAllText(FullPath, json);
        }

        public static BatchInjectorData LoadOrCreateData()
        {
            if (!File.Exists(FullPath))
                return new BatchInjectorData();

            BatchInjectorData data;

            try
            {
                string json = File.ReadAllText(FullPath);
                data = JsonUtility.FromJson<BatchInjectorData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Batch Injector: Failed to load configuration data. {e.Message} Creating new file.");
                data = new BatchInjectorData();
                SaveData(data);
            }

            return data;
        }
    }
}