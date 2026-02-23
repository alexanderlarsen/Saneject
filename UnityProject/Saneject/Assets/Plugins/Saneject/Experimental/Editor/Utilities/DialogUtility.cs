using System.Collections.Generic;
using System.Linq;
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
            public static bool Confirm_InjectCurrentScene(
                string sceneName,
                ContextWalkFilter walkFilter)
            {
                if (!UserSettings.AskBefore_Inject_Scene)
                    return true;

                string walkFilterString = ObjectNames.NicifyVariableName(walkFilter.ToString());

                return EditorUtility.DisplayDialog
                (
                    title: $"Saneject: Inject Current Scene ({walkFilterString})",
                    message: walkFilter switch
                    {
                        ContextWalkFilter.AllContexts => $"Are you sure you want to inject the entire {sceneName} scene, including scene objects and prefab instances?",
                        ContextWalkFilter.SceneObjects => $"Are you sure you want to inject all scene objects in the {sceneName} scene, excluding prefab instances?",
                        ContextWalkFilter.PrefabInstances => $"Are you sure you want to inject all prefab instances in the {sceneName} scene, excluding scene objects?",
                        ContextWalkFilter.SameContextsAsSelection => $"Are you sure you want to inject all objects in the {sceneName} scene that are the same context as the selection?",
                        _ => $"The {walkFilter} context walk filter is not supported for scene injection."
                    },
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_InjectCurrentPrefabAsset(
                string prefabName,
                ContextWalkFilter walkFilter)
            {
                if (!UserSettings.AskBefore_Inject_PrefabAsset)
                    return true;

                string walkFilterString = ObjectNames.NicifyVariableName(walkFilter.ToString());

                return EditorUtility.DisplayDialog
                (
                    title: $"Saneject: Inject Prefab Asset ({walkFilterString})",
                    message: walkFilter switch
                    {
                        ContextWalkFilter.AllContexts => $"Are you sure you want to inject the entire {prefabName} prefab asset, including prefab asset objects and prefab instances?",
                        ContextWalkFilter.PrefabAssetObjects => $"Are you sure you want to inject all prefab asset objects in the {prefabName} prefab asset, excluding prefab instances?",
                        ContextWalkFilter.PrefabInstances => $"Are you sure you want to inject all prefab instances in the {prefabName} prefab asset, excluding prefab asset objects?",
                        ContextWalkFilter.SameContextsAsSelection => $"Are you sure you want to inject all objects in the {prefabName} prefab asset that are the same context as the selection?",
                        _ => "Are you sure you want to inject the selected hierarchy?"
                    },
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_InjectSceneHierarchy(ContextWalkFilter walkFilter)
            {
                if (!UserSettings.AskBefore_Inject_SelectedSceneHierarchies)
                    return true;

                string walkFilterString = ObjectNames.NicifyVariableName(walkFilter.ToString());

                return EditorUtility.DisplayDialog
                (
                    title: $"Saneject: Inject Scene Hierarchy ({walkFilterString})",
                    message: walkFilter switch
                    {
                        ContextWalkFilter.AllContexts => "Are you sure you want to inject all objects in the selected scene hierarchy, including scene objects and prefab instances?",
                        ContextWalkFilter.SceneObjects => "Are you sure you want to inject all scene objects in the selected scene hierarchy, excluding prefab instances?",
                        ContextWalkFilter.PrefabInstances => "Are you sure you want to inject all prefab instances in the selected scene hierarchy, excluding scene objects?",
                        ContextWalkFilter.SameContextsAsSelection => "Are you sure you want to inject all objects in the selected scene hierarchy that are the same context as the selected object?",
                        _ => "Are you sure you want to inject the selected scene hierarchy?"
                    },
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_InjectSelectedSceneHierarchies(ContextWalkFilter walkFilter)
            {
                if (!UserSettings.AskBefore_Inject_SelectedSceneHierarchies)
                    return true;

                string walkFilterString = ObjectNames.NicifyVariableName(walkFilter.ToString());

                return EditorUtility.DisplayDialog
                (
                    title: $"Saneject: Inject Selected Scene Hierarchies ({walkFilterString})",
                    message: walkFilter switch
                    {
                        ContextWalkFilter.AllContexts => "Are you sure you want to inject all objects in the selected scene hierarchies, including scene objects and prefab instances?",
                        ContextWalkFilter.SceneObjects => "Are you sure you want to inject all scene objects in the selected scene hierarchies, excluding prefab instances?",
                        ContextWalkFilter.PrefabInstances => "Are you sure you want to inject all prefab instances in the selected scene hierarchies, excluding scene objects?",
                        ContextWalkFilter.SameContextsAsSelection => "Are you sure you want to inject all objects in the selected scene hierarchies that are the same context as the selected object?",
                        _ => "Are you sure you want to inject the selected scene hierarchies?"
                    },
                    ok: "Inject",
                    cancel: "Cancel"
                );
            }
        }

        public static class BatchInjectionMenus
        {
            public static bool Confirm_BatchInjector_Inject(
                int sceneCount,
                int prefabCount)
            {
                return Confirm_BatchInject(sceneCount, prefabCount, true);
            }

            public static bool Confirm_BatchInject_SelectedAssets(
                int sceneCount,
                int prefabCount)
            {
                return Confirm_BatchInject(sceneCount, prefabCount, false);
            }

            private static bool Confirm_BatchInject(
                int sceneCount,
                int prefabCount,
                bool isBatchInjectorEditorWindow)
            {
                if (!UserSettings.AskBefore_BatchInject)
                    return true;

                StringBuilder sb = new();

                sb.Append("You are about to batch inject ");

                if (sceneCount > 0)
                {
                    sb.Append($"{sceneCount} {(sceneCount == 1 ? "scene" : "scenes")}");

                    if (prefabCount > 0)
                        sb.Append(" and ");
                }

                if (prefabCount > 0)
                    sb.Append($"{prefabCount} {(prefabCount == 1 ? "prefab" : "prefabs")}");

                sb.AppendLine(".");
                sb.AppendLine();
                sb.AppendLine("Injection will be performed on all selected assets that contain one or more scopes.");
                sb.AppendLine();

                sb.AppendLine
                (
                    isBatchInjectorEditorWindow
                        ? "Each asset will be injected using its context walk filter configured in the Batch Injector."
                        : "Assets will be fully injected, including scene objects, prefab instances, and prefab assets."
                );

                sb.AppendLine();
                sb.AppendLine("Do you want to continue?");

                return EditorUtility.DisplayDialog
                (
                    title: isBatchInjectorEditorWindow
                        ? "Saneject: Batch Inject Selected Assets"
                        : "Saneject: Batch Inject Selected Assets (All Contexts)",
                    message: sb.ToString(),
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
                    message: $"One or more FromRuntimeProxy() bindings {(proxyCount == 1 ? $"needs {proxyCount} runtime proxy script" : $"need {proxyCount} runtime proxy scripts")}.\n\n" +
                             $"{(proxyCount == 1 ? "It" : "They")} will be generated during this domain reload and saved to:\n\n" +
                             $"{ProjectSettings.ProxyAssetGenerationFolder}\n\n" +
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

        public static class ProxyCleaner
        {
            public static void Display_NoUnusedProxies()
            {
                EditorUtility.DisplayDialog
                (
                    title: "Saneject: Delete Unused Runtime Proxies",
                    message: "Your project is already clean. No unused runtime proxy assets or scripts were found.",
                    ok: "Got it"
                );
            }

            public static bool Confirm_DeleteAssets(
                int assetCount,
                IEnumerable<string> unusedTypeNames)
            {
                unusedTypeNames = unusedTypeNames.ToHashSet();

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Delete Unused Runtime Proxies",
                    message: $"Found {assetCount} unused runtime proxy {(assetCount == 1 ? "asset" : "assets")} for {(unusedTypeNames.Count() == 1 ? "type" : "types")}:\n\n" +
                             $"{string.Join(", ", unusedTypeNames)}\n\n" +
                             $"Do you want to delete {(assetCount == 1 ? "it" : "them")}?",
                    ok: "Delete",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_DeleteScripts(IEnumerable<string> unusedTypeNames)
            {
                unusedTypeNames = unusedTypeNames.ToHashSet();
                int count = unusedTypeNames.Count();

                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Delete Unused Runtime Proxies",
                    message: $"Found {count} unused runtime proxy {(count == 1 ? "script" : "scripts")}:\n\n" +
                             $"{string.Join(", ", unusedTypeNames)}\n\n" +
                             $"Do you want to delete {(count == 1 ? "it" : "them")}?",
                    ok: "Delete",
                    cancel: "Cancel"
                );
            }
        }

        public static class Settings
        {
            public static bool Confirm_UseDefaultUserSettings()
            {
                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Use Default User Settings",
                    message: "Are you sure you want to reset the Saneject user settings to their default values?",
                    ok: "Reset",
                    cancel: "Cancel"
                );
            }

            public static bool Confirm_UseDefaultProjectSettings()
            {
                return EditorUtility.DisplayDialog
                (
                    title: "Saneject: Use Default Project Settings",
                    message: "Are you sure you want to reset the Saneject project settings to their default values?\n\n" +
                             "Warning: These settings are project-wide and affect all users of this project.",
                    ok: "Reset",
                    cancel: "Cancel"
                );
            }
        }
    }
}