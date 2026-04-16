using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Data;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SceneListUtility
    {
        public static void AddOpenScenes(BatchInjectorData batchInjectorData)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (string.IsNullOrEmpty(scene.path) || !scene.path.EndsWith(".unity"))
                    continue;

                batchInjectorData.SceneList.TryAddAssetByPath<SceneAssetData>(scene.path);
            }

            batchInjectorData.SceneList.Sort();
            batchInjectorData.IsDirty = true;
        }

        public static void AddAllProjectScenes(BatchInjectorData batchInjectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[]
            {
                "Assets"
            });

            string[] newGuids = batchInjectorData.SceneList.FindGuidsNotInList(guids).ToArray();

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
                    batchInjectorData.SceneList.TryAddAssetByGuid<SceneAssetData>(guid);

                batchInjectorData.SceneList.Sort();
                batchInjectorData.IsDirty = true;
            }
        }

        public static void ClearScenes(BatchInjectorData batchInjectorData)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            batchInjectorData.SceneList.Clear();
            batchInjectorData.IsDirty = true;
        }
    }
}