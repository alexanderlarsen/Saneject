using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    public static class PrefabListManager
    {
        public static void AddAllPrefabsInScene(BatchInjectorData data)
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
                data.prefabs.TryAdd(path);

            data.prefabs.Sort();
            Storage.SaveData(data);
        }

        public static void AddAllProjectPrefabs(BatchInjectorData data)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            List<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && p.EndsWith(".prefab"))
                .ToList();

            List<string> newPrefabs = data.prefabs.GetAssetPathsNotInList(paths);

            if (newPrefabs.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    title: "Batch Injector",
                    message: "All prefabs are already added.",
                    ok: "OK"
                );

                return;
            }

            if (EditorUtility.DisplayDialog(
                    title: "Batch Injector",
                    message: $"Do you want to add {newPrefabs.Count} prefab{(newPrefabs.Count == 1 ? "" : "s")} to the Batch Injector?",
                    ok: "Yes",
                    cancel: "No"
                ))
            {
                foreach (string path in newPrefabs)
                    data.prefabs.TryAdd(path);

                data.prefabs.Sort();
                Storage.SaveData(data);
            }
        }

        public static void ClearPrefabs(BatchInjectorData data)
        {
            if (!EditorUtility.DisplayDialog(
                    title: "Batch Injector",
                    message: "Remove all prefabs from list?",
                    ok: "Yes",
                    cancel: "Cancel"
                ))
                return;

            data.prefabs.Clear();
            Storage.SaveData(data);
        }
    }
}