using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Persistence;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class SceneListUtility
    {
        public static void AddOpenScenes(BatchInjectorData data)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (string.IsNullOrEmpty(scene.path) || !scene.path.EndsWith(".unity"))
                    continue;

                data.sceneList.TryAddAssetByPath(scene.path);
            }

            data.sceneList.Sort();
            data.isDirty = true;
        }

        public static void AddAllProjectScenes(BatchInjectorData data)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[]
            {
                "Assets"
            });

            string[] newGuids = data.sceneList.FindGuidsNotInList(guids).ToArray();

            if (newGuids.Length == 0)
            {
                EditorUtility.DisplayDialog("Batch Injector",
                    "All scenes are already added.", "OK");

                return;
            }

            if (EditorUtility.DisplayDialog("Batch Injector",
                    $"Do you want to add {newGuids.Length} scene{(newGuids.Length == 1 ? "" : "s")} to the Batch Injector?",
                    "Yes", "No"))
            {
                foreach (string guid in newGuids)
                    data.sceneList.TryAddAssetByGuid(guid);

                data.sceneList.Sort();
                data.isDirty = true;
            }
        }

        public static void ClearScenes(BatchInjectorData data)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            data.sceneList.Clear();
            data.isDirty = true;
        }
    }
}