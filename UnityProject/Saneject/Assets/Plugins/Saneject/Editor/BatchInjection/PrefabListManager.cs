using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.BatchInjection
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
                data.prefabList.TryAddByPath(path);

            data.prefabList.Sort();
            Storage.SaveData(data);
        }

        public static void AddAllProjectPrefabs(BatchInjectorData data)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            string[] newGuids = data.prefabList.FindGuidsNotInList(guids).ToArray();

            if (newGuids.Length == 0)
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
                    message: $"Do you want to add {newGuids.Length} prefab{(newGuids.Length == 1 ? "" : "s")} to the Batch Injector?",
                    ok: "Yes",
                    cancel: "No"
                ))
            {
                foreach (string guid in newGuids)
                    data.prefabList.TryAddByGuid(guid);

                data.prefabList.Sort();
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

            data.prefabList.Clear();
            Storage.SaveData(data);
        }
    }
}