using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class PrefabListUtility
    {
        public static void AddAllPrefabsInScene(BatchInjectorData batchInjectorData)
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
                batchInjectorData.prefabList.TryAddAssetByPath<PrefabAssetData>(path);

            batchInjectorData.prefabList.Sort();
            batchInjectorData.isDirty = true;
        }

        public static void AddAllProjectPrefabs(BatchInjectorData batchInjectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[]
            {
                "Assets"
            });

            string[] newGuids = batchInjectorData.prefabList.FindGuidsNotInList(guids).ToArray();

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
                    batchInjectorData.prefabList.TryAddAssetByGuid<PrefabAssetData>(guid);

                batchInjectorData.prefabList.Sort();
                batchInjectorData.isDirty = true;
            }
        }

        public static void ClearPrefabs(BatchInjectorData batchInjectorData)
        {
            if (!EditorUtility.DisplayDialog
                (
                    title: "Batch Injector",
                    message: "Remove all prefabs from list?",
                    ok: "Yes",
                    cancel: "Cancel"
                ))
                return;

            batchInjectorData.prefabList.Clear();
            batchInjectorData.isDirty = true;
        }
    }
}