using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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
                batchInjectorData.PrefabList.TryAddAssetByPath<PrefabAssetData>(path);

            batchInjectorData.PrefabList.Sort();
            batchInjectorData.IsDirty = true;
        }

        public static void AddAllProjectPrefabs(BatchInjectorData batchInjectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[]
            {
                "Assets"
            });

            string[] newGuids = batchInjectorData.PrefabList.FindGuidsNotInList(guids).ToArray();

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
                    batchInjectorData.PrefabList.TryAddAssetByGuid<PrefabAssetData>(guid);

                batchInjectorData.PrefabList.Sort();
                batchInjectorData.IsDirty = true;
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

            batchInjectorData.PrefabList.Clear();
            batchInjectorData.IsDirty = true;
        }
    }
}