using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    public static class PrefabActions
    {
        public static void AddAllPrefabsInScene(
            BatchInjectData data,
            SortMode sortMode)
        {
            HashSet<string> prefabsInScene = new();

            foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                
                if (!prefab) 
                    continue;

                string path = AssetDatabase.GetAssetPath(prefab);
                
                if (string.IsNullOrEmpty(path)) 
                    continue;
                
                prefabsInScene.Add(path);
            }

            foreach (string path in prefabsInScene)
                if (data.prefabs.All(p => p.path != path))
                    data.prefabs.Add(new AssetEntry(path));

            AssetListSorter.SortList(data.prefabs, sortMode);
            Storage.SaveData(data);
        }

        public static void AddAllProjectPrefabs(BatchInjectData data, SortMode sortMode)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            List<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && p.EndsWith(".prefab"))
                .ToList();

            List<string> newPrefabs = paths
                .Where(p => data.prefabs.All(s => s.path != p))
                .Where(p => AssetDatabase.LoadAssetAtPath<GameObject>(p))
                .ToList();

            if (newPrefabs.Count == 0)
            {
                EditorUtility.DisplayDialog("Batch Injector", "All prefabs are already added.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Batch Injector",
                    $"Do you want to add {newPrefabs.Count} prefab{(newPrefabs.Count == 1 ? "" : "s")} to the Batch Injector?",
                    "Yes", "No"))
            {
                foreach (string path in newPrefabs)
                    data.prefabs.Add(new AssetEntry(path));

                AssetListSorter.SortList(data.prefabs, sortMode);
                Storage.SaveData(data);
            }
        }

        public static void ClearPrefabs(BatchInjectData data)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all prefabs from list?", "Yes", "Cancel"))
                return;

            data.prefabs.Clear();
            Storage.SaveData(data);
        }
    }
}