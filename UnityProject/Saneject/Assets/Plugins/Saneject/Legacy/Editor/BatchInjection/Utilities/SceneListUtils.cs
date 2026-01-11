using System.Linq;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Data;
using Plugins.Saneject.Legacy.Editor.BatchInjection.Persistence;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Legacy.Editor.BatchInjection.Utilities
{
    public static class SceneListUtils
    {
        public static void AddOpenScenes(BatchInjectorData injectorData)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (string.IsNullOrEmpty(scene.path) || !scene.path.EndsWith(".unity"))
                    continue;

                injectorData.sceneList.TryAddByPath(scene.path);
            }

            injectorData.sceneList.Sort();
            Storage.SaveData(injectorData);
        }

        public static void AddAllProjectScenes(BatchInjectorData injectorData)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
            string[] newGuids = injectorData.sceneList.FindGuidsNotInList(guids).ToArray();

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
                    injectorData.sceneList.TryAddByGuid(guid);

                injectorData.sceneList.Sort();
                Storage.SaveData(injectorData);
            }
        }

        public static void ClearScenes(BatchInjectorData injectorData)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            injectorData.sceneList.Clear();
            Storage.SaveData(injectorData);
        }
    }
}