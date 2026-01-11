using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Data;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Persistence;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Legacy.Editor.BatchInjection.Utilities
{
    public static class PrefabListUtils
    {
        public static void AddAllPrefabsInScene(BatchInjectorData injectorData)
        {
            HashSet<string> paths = new();

            foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);

                if (!prefab)
                    continue;

                string path = AssetDatabase.GetAssetPath(prefab);

                if (string.IsNullOrEmpty(path))
                    continue;

                paths.Add(path);
            }

            foreach (string path in paths)
                injectorData.prefabList.TryAddByPath(path);

            injectorData.prefabList.Sort();
            Storage.SaveData(injectorData);
        }

        public static void AddAllProjectPrefabs(BatchInjectorData injectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            string[] newGuids = injectorData.prefabList.FindGuidsNotInList(guids).ToArray();

            if (newGuids.Length == 0)
            {
                EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: "All prefabs are already added.",
                    ok: "OK"
                );

                return;
            }

            if (EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: $"Do you want to add {newGuids.Length} prefab{(newGuids.Length == 1 ? "" : "s")} to the Batch Injector?",
                    ok: "Yes",
                    cancel: "No"
                ))
            {
                foreach (string guid in newGuids)
                    injectorData.prefabList.TryAddByGuid(guid);

                injectorData.prefabList.Sort();
                Storage.SaveData(injectorData);
            }
        }

        public static void ClearPrefabs(BatchInjectorData injectorData)
        {
            if (!EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: "Remove all prefabs from list?",
                    ok: "Yes",
                    cancel: "Cancel"
                ))
                return;

            injectorData.prefabList.Clear();
            Storage.SaveData(injectorData);
        }
    }
}