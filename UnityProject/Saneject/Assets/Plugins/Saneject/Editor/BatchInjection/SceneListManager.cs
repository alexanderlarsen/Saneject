using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.BatchInjection
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

                data.sceneList.TryAddByPath(scene.path);
            }

            data.sceneList.Sort();
            Storage.SaveData(data);
        }

        public static void AddAllProjectScenes(BatchInjectorData data)
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
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
                    data.sceneList.TryAddByGuid(guid);

                data.sceneList.Sort();
                Storage.SaveData(data);
            }
        }

        public static void ClearScenes(BatchInjectorData data)
        {
            if (!EditorUtility.DisplayDialog("Batch Injector", "Remove all scenes from list?", "Yes", "Cancel"))
                return;

            data.sceneList.Clear();
            Storage.SaveData(data);
        }
    }
}