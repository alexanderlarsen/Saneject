using Plugins.Saneject.Runtime.Settings;
using UnityEditor;

namespace Plugins.Saneject.Editor.Utility
{
    public static class Dialogs
    {
        public static class Injection
        {
            public static bool ConfirmInjectHierarchy()
            {
                if (!UserSettings.AskBeforeHierarchyInjection)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Inject Hierarchy Dependencies",
                    message: "Are you sure you want to inject all dependencies in the hierarchy?",
                    ok: "Yes",
                    cancel: "Cancel");
            }

            public static bool ConfirmInjectScene()
            {
                if (!UserSettings.AskBeforeSceneInjection)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Inject Scene Dependencies",
                    message: "Are you sure you want to inject all dependencies in the scene?",
                    ok: "Yes",
                    cancel: "Cancel");
            }

            public static bool ConfirmInjectPrefab()
            {
                if (!UserSettings.AskBeforePrefabInjection)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Inject Prefab Dependencies",
                    message: "Are you sure you want to inject all dependencies in the prefab?",
                    ok: "Yes",
                    cancel: "Cancel");
            }
        }

        public static class BatchInjection
        {
            public static bool ConfirmInjectAll(
                int sceneCount,
                int prefabCount)
            {
                if (!UserSettings.AskBeforeBatchInjectAll)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Batch Inject All Dependencies",
                    message: $"Are you sure you want to batch inject {sceneCount} scenes and {prefabCount} prefabs?",
                    ok: "Yes",
                    cancel: "Cancel");
            }

            public static bool ConfirmInjectScene(int sceneCount)
            {
                if (!UserSettings.AskBeforeBatchInjectScenes)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Batch Inject Scene Dependencies",
                    message: $"Are you sure you want to batch inject {sceneCount} scenes?",
                    ok: "Yes",
                    cancel: "Cancel");
            }

            public static bool ConfirmInjectPrefab(int prefabCount)
            {
                if (!UserSettings.AskBeforeBatchInjectPrefabs)
                    return true;

                return EditorUtility.DisplayDialog(
                    title: "Saneject: Batch Inject Prefab Dependencies",
                    message: $"Are you sure you want to batch inject {prefabCount} prefabs?",
                    ok: "Yes",
                    cancel: "Cancel");
            }
        }
    }
}