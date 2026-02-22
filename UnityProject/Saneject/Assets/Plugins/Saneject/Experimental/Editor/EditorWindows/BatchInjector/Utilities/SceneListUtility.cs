using System.Linq;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Utilities
{
    public static class SceneListUtility
    {
        public static void AddOpenScenes(BatchInjectorData batchInjectorData)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (string.IsNullOrEmpty(scene.path) || !scene.path.EndsWith(".unity"))
                    continue;

                batchInjectorData.sceneList.TryAddAssetByPath<SceneAssetData>(scene.path);
            }

            batchInjectorData.sceneList.Sort();
            batchInjectorData.isDirty = true;
        }

        public static void AddAllProjectScenes(BatchInjectorData batchInjectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[]
            {
                "Assets"
            });

            string[] newGuids = batchInjectorData.sceneList.FindGuidsNotInList(guids).ToArray();

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
                    batchInjectorData.sceneList.TryAddAssetByGuid<SceneAssetData>(guid);

                batchInjectorData.sceneList.Sort();
                batchInjectorData.isDirty = true;
            }
        }

        public static void ClearScenes(BatchInjectorData batchInjectorData)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            batchInjectorData.sceneList.Clear();
            batchInjectorData.isDirty = true;
        }
    }
}