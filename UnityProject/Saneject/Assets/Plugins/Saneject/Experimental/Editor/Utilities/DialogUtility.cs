using System.Text;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class DialogUtility
    {
        public static class InjectionPipeline
        {
            public static void Display_EditorOnlyInjection()
            {
                EditorUtility.DisplayDialog
                (
                    title: "Saneject",
                    message: "Injection is editor-only. Exit Play Mode to inject.",
                    ok: "Got it"
                );
            }
        }

        public static class InjectionMenus
        {
            public static bool Confirm_Inject_Scene(string sceneName)
            {
                if (!UserSettings.AskBefore_Inject_CurrentScene_Or_CurrentPrefab)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject Scene",
                    message: $"Are you sure you want to inject the entire {sceneName} scene, including prefabs instances?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_Inject_PrefabAsset(string prefabName)
            {
                if (!UserSettings.AskBefore_Inject_CurrentScene_Or_CurrentPrefab)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject Prefab",
                    message: $"Are you sure you want to inject the {prefabName} prefab?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_Inject_AllSceneObjects(string sceneName)
            {
                if (!UserSettings.AskBefore_Inject_AllSceneObjects_Or_AllScenePrefabInstances)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject All Scene Objects",
                    message: $"Are you sure you want to inject all scene objects in the {sceneName} scene?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_Inject_AllScenePrefabInstances(string sceneName)
            {
                if (!UserSettings.AskBefore_Inject_AllSceneObjects_Or_AllScenePrefabInstances)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject All Scene Prefab Instances",
                    message: $"Are you sure you want to inject all prefab instances in the {sceneName} scene?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_Inject_SelectedSceneHierarchies_AllContexts()
            {
                if (!UserSettings.AskBefore_Inject_SelectedSceneHierarchies)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject Selected Scene Hierarchies (All Contexts)",
                    message: "Are you sure you want to inject all scene objects and prefab instances in the selected scene hierarchies?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool SelectedSceneHierarchies_SelectedObjectContextsOnly()
            {
                if (!UserSettings.AskBefore_Inject_SelectedSceneHierarchies)
                    return true;

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject Selected Scene Hierarchies (Selected Object Contexts Only)",
                    message: "Are you sure you want to inject the selected hierarchies filtered by the selected object contexts?",
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_Inject_Single_SceneHierarchy_ByContext(ContextWalkFilter walkFilter)
            {
                if (!UserSettings.AskBefore_Inject_SelectedSceneHierarchies)
                    return true;

                string walkFilterString = ObjectNames.NicifyVariableName(walkFilter.ToString()).ToLower();
                string message;

                if (walkFilter == ContextWalkFilter.All)
                    message = "Are you sure you want to inject all scene objects and prefab instances in the selected scene hierarchy?";
                else if (walkFilter == ContextWalkFilter.SameAsStartObjects)
                    message = "Are you sure you want to inject the selected hierarchy filtered by the same object contexts as the selected object?";
                else
                    message = $"Are you sure you want to inject the selected hierarchy filtered by {walkFilterString}?";
                
                return EditorUtility.DisplayDialog
                (
                    title:
                    walkFilter == ContextWalkFilter.All
                        ? "Saneject: Inject Selected Scene Hierarchy (All)"
                        : $"Saneject: Inject Selected Scene Hierarchy ({walkFilterString})",
                    message: message,
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }
        }

        public static class BatchInjectionMenus
        {
            public static bool Confirm_BatchInject_SelectedAssets(
                int sceneCount,
                int prefabCount)
            {
                if (!UserSettings.AskBefore_BatchInject_SelectedAssets)
                    return true;

                StringBuilder messageBuilder = new();

                messageBuilder.Append("Your selection includes ");

                if (sceneCount > 0)
                {
                    messageBuilder.Append($"{sceneCount} {(sceneCount == 1 ? "scene" : "scenes")}");

                    if (prefabCount > 0)
                        messageBuilder.Append(" and ");
                }

                if (prefabCount > 0)
                    messageBuilder.Append($"{prefabCount} {(prefabCount == 1 ? "prefab" : "prefabs")}");

                messageBuilder.Append(".");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine();
                messageBuilder.AppendLine("A batch injection operation will be performed on all selected assets that contain at least one scope.");
                messageBuilder.AppendLine();
                messageBuilder.Append("Do you want to continue?");

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Inject Selected Assets (Batch Injection)",
                    message: messageBuilder.ToString(),
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }
        }

        public static class ProxyGeneration
        {
            public static void Display_ProxyCreate(int proxyCount)
            {
                EditorUtility.DisplayDialog
                (
                    title: "Saneject: Runtime Proxy Generation",
                    message: $"{proxyCount} of your FromRuntimeProxy() bindings {(proxyCount == 1 ? "needs a proxy script" : "need proxy scripts")}.\n\n" +
                             $"{(proxyCount == 1 ? "It" : "They")} will be generated during this domain reload and saved to:\n\n" +
                             $"{UserSettings.ProxyAssetGenerationFolder}\n\n" +
                             "You can disable automatic proxy generation in the Saneject settings and run it manually from the Saneject menu instead.",
                    ok: "Got it"
                );
            }

            public static void Display_ProxyAlreadyExist()
            {
                EditorUtility.DisplayDialog
                (
                    title: "Saneject: Runtime Proxy Generation",
                    message: "All runtime proxy scripts needed for your FromRuntimeProxy() bindings already exist.",
                    ok: "Got it"
                );
            }
        }

        public static class Settings
        {
            public static bool Confirm_UseDefaultSettings()
            {
                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Use Default Settings",
                    message: "Are you sure you want to reset the Saneject settings to their default values?",
                    ok: "Reset",
                    cancel: "Cancel"
                );
            }
        }
    }
}