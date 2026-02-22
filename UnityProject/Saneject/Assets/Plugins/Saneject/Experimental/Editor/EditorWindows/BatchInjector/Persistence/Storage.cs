using System;
using System.IO;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Persistence
{
    public static class Storage
    {
        private static readonly string Folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings/Saneject"));
        private static readonly string FullPath = Path.Combine(Folder, "BatchInjectorData.json");

        public static void SaveIfDirty(BatchInjectorData data)
        {
            if (!data.isDirty)
                return;

            Save(data);
            data.isDirty = false;
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
                Debug.LogError($"Saneject: Batch Injector failed to load configuration data. {e.Message} Creating new file.");
                data = new BatchInjectorData();
                Save(data);
            }

            return data;
        }

        private static void Save(BatchInjectorData data)
        {
            Directory.CreateDirectory(Folder);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(FullPath, json);
        }
    }
}