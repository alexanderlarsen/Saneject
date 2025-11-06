using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    public static class SceneListManager
    {
        public static void AddOpenScenes(BatchInjectorData data)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (string.IsNullOrEmpty(scene.path) || !scene.path.EndsWith(".unity"))
                    continue;

                data.scenes.TryAdd(scene.path);
            }

            data.scenes.Sort();
            Storage.SaveData(data);
        }

        public static void AddAllProjectScenes(BatchInjectorData data)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

            List<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && p.EndsWith(".unity"))
                .ToList();

            List<string> newScenes = data.scenes.GetAssetPathsNotInList(paths);

            if (newScenes.Count == 0)
            {
                EditorUtility.DisplayDialog("Batch Injector",
                    "All scenes are already added.", "OK");

                return;
            }

            if (EditorUtility.DisplayDialog("Batch Injector",
                    $"Do you want to add {newScenes.Count} scene{(newScenes.Count == 1 ? "" : "s")} to the Batch Injector?",
                    "Yes", "No"))
            {
                foreach (string path in newScenes)
                    data.scenes.TryAdd(path);

                data.scenes.Sort();
                Storage.SaveData(data);
            }
        }

        public static void ClearScenes(BatchInjectorData data)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            data.scenes.Clear();
            Storage.SaveData(data);
        }
    }
}