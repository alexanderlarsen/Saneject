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

        public static void SaveIfDirty(BatchInjectorData batchInjectorData)
        {
            if (!batchInjectorData.isDirty)
                return;

            Save(batchInjectorData);
            batchInjectorData.isDirty = false;
        }

        public static BatchInjectorData LoadOrCreateData()
        {
            if (!File.Exists(FullPath))
                return new BatchInjectorData();

            BatchInjectorData batchInjectorData;

            try
            {
                string json = File.ReadAllText(FullPath);
                batchInjectorData = JsonUtility.FromJson<BatchInjectorData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Saneject: Batch Injector failed to load configuration data. {e.Message} Creating new file.");
                batchInjectorData = new BatchInjectorData();
                Save(batchInjectorData);
            }

            return batchInjectorData;
        }

        private static void Save(BatchInjectorData batchInjectorData)
        {
            Directory.CreateDirectory(Folder);
            string json = JsonUtility.ToJson(batchInjectorData, prettyPrint: true);
            File.WriteAllText(FullPath, json);
        }
    }
}