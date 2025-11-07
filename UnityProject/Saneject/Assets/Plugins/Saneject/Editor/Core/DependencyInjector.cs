using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Plugins.Saneject.Editor.BatchInjection;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Plugins.Saneject.Editor.Core
{
    /// <summary>
    /// Provides editor-only dependency injection for Saneject-enabled scenes and prefabs.
    /// Handles resolving and assigning dependencies for all <see cref="Scope" />s in the current scene or prefab,
    /// based on the binding strategies and filters configured by the user.
    /// </summary>
    public static class DependencyInjector
    {
        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a hierarchy root in the scene.
        /// Scans for configured bindings starting from the given scope, resolves them recursively up the hierarchy,
        /// and assigns references to all eligible fields/properties.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope to start injection from. All child scopes will be processed.</param>
        public static void InjectSingleHierarchy(Scope startScope)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            RunInjectionPass
            (
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(includeInactive: true)
                    .Where(scope => !scope.gameObject.IsPrefab())
                    .ToArray(),
                noScopesWarning: "Saneject: No scopes found in hierarchy. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting all objects in hierarchy",
                configureGlobalBindings: true,
                isPrefabInjection: false,
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Single scene hierarchy injection completed [{rootName}]";
                }
            );
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a prefab root.
        /// Only used for prefab assets - does not register or inject global dependencies.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope in the prefab to start injection from.</param>
        public static void InjectPrefab(Scope startScope)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            if (!startScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            RunInjectionPass
            (
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(includeInactive: true),
                noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting prefab dependencies",
                configureGlobalBindings: false,
                isPrefabInjection: true,
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Prefab injection completed [{rootName}]";
                }
            );
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s in the currently active scene.
        /// Scans all non-prefab root objects and their descendants for scopes, processes bindings,
        /// and injects dependencies where applicable.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        public static void InjectCurrentScene()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();

            RunInjectionPass
            (
                collectScopes: () => scene
                    .GetRootGameObjects()
                    .Where(root => !root.IsPrefab())
                    .SelectMany(root => root.GetComponentsInChildren<Scope>(includeInactive: true))
                    .ToArray(),
                noScopesWarning: "Saneject: No scopes found in scene. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting all objects in scene",
                configureGlobalBindings: true,
                isPrefabInjection: false,
                buildStatsLabel: _ => $"Scene injection completed [{scene.name}]"
            );
        }

        /// <summary>
        /// Performs dependency injection across multiple prefabs in sequence.
        /// Each prefab asset is loaded, processed for <see cref="Scope" />s, injected, and saved back to disk.
        /// Logs progress per prefab and aggregates statistics for all processed scopes.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="prefabAssets">Array of prefab asset items to inject, in order of processing.</param>
        /// <param name="logStats">Whether to log the summary statistics after completion.</param>
        /// <param name="stats">Optional <see cref="InjectionStats" /> object to accumulate results into. A new one is created if omitted.</param>
        public static void BatchInjectPrefabs(
            AssetItem[] prefabAssets,
            bool logStats,
            InjectionStats stats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            string previousScenePath = SceneManager.GetActiveScene().path;

            if (ProxyUtils.TryCreateProxyScriptsForPrefabs(prefabAssets.Select(prefab => prefab.Path)))
            {
                EditorSceneManager.OpenScene(previousScenePath);
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            stats ??= new InjectionStats();

            Debug.Log($"<b>──────────  Saneject: Starting prefab batch injection of {prefabAssets.Length} prefab{(prefabAssets.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();
            int numScopesProcessed = 0;

            for (int i = 0; i < prefabAssets.Length; i++)
            {
                AssetItem prefabItem = prefabAssets[i];
                string path = prefabItem.Path;
                string prefabName = Path.GetFileNameWithoutExtension(path);
                string logSectionColor = EditorColors.BatchLogColors[i % EditorColors.BatchLogColors.Length];

                prefabItem.Status = InjectionStatus.Unknown;

                Debug.Log($"<color={logSectionColor}><b>↓↓↓</b> Saneject: Start prefab injection [{prefabName}] <b>↓↓↓</b></color>");

                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (!prefabAsset)
                {
                    Debug.LogWarning($"Saneject: Failed to load prefab asset at '{path}'. Skipping.");
                    Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End prefab injection [{prefabName}] <b>↑↑↑</b></color>");
                    continue;
                }

                Scope[] scopes = prefabAsset.GetComponentsInChildren<Scope>(true);

                if (scopes.Length == 0)
                {
                    Debug.LogWarning($"Saneject: No scopes found in prefab '{prefabName}'. Nothing to inject.");
                    Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End prefab injection [{prefabName}] <b>↑↑↑</b></color>");
                    continue;
                }

                RunInjectionPass(
                    collectScopes: () => scopes,
                    noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                    progressBarTitle: "Saneject: Injection in progress",
                    progressBarMessage: $"Injecting prefab dependencies ({prefabName})",
                    configureGlobalBindings: false,
                    isPrefabInjection: true,
                    buildStatsLabel: _ => $"Prefab injection completed [{prefabName}]",
                    addResultToStats: stats,
                    assetItem: prefabItem
                );

                EditorUtility.SetDirty(prefabAsset);
                AssetDatabase.SaveAssets();
                numScopesProcessed += scopes.Length;
                Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End prefab injection [{prefabName}] <b>↑↑↑</b></color>");
            }

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stats.numScopesProcessed = numScopesProcessed;
            stopwatch.Stop();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(firstSentence: $"Prefab batch injection complete | Processed {prefabAssets.Length} prefabs");
        }

        /// <summary>
        /// Performs dependency injection across multiple scenes in sequence.
        /// Each scene is opened, injected, and saved before continuing to the next.
        /// Logs progress per scene and aggregates statistics for all processed scopes.
        /// Restores the originally active scene after completion.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="sceneAssets">Array of scene asset items to inject, in order of processing.</param>
        /// <param name="logStats">Whether to log the summary statistics after completion.</param>
        /// <param name="stats">Optional <see cref="InjectionStats" /> object to accumulate results into. A new one is created if omitted.</param>
        public static void BatchInjectScenes(
            AssetItem[] sceneAssets,
            bool logStats,
            InjectionStats stats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            string previousScenePath = SceneManager.GetActiveScene().path;

            if (ProxyUtils.TryCreateProxyScriptsForScenes(sceneAssets.Select(scene => scene.Path)))
            {
                EditorSceneManager.OpenScene(previousScenePath);
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            stats ??= new InjectionStats();

            Debug.Log($"<b>──────────  Saneject: Start scene batch injection of {sceneAssets.Length} scene{(sceneAssets.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();
            int numScopesProcessed = 0;

            for (int i = 0; i < sceneAssets.Length; i++)
            {
                AssetItem sceneItem = sceneAssets[i];
                Scene scene = EditorSceneManager.OpenScene(sceneItem.Path, OpenSceneMode.Single);
                string logSectionColor = EditorColors.BatchLogColors[i % EditorColors.BatchLogColors.Length];
                sceneItem.Status = InjectionStatus.Unknown;

                Debug.Log($"<color={logSectionColor}><b>↓↓↓</b> Saneject: Start scene injection [{scene.name}] <b>↓↓↓</b></color>");

                Scope[] scopes = scene
                    .GetRootGameObjects()
                    .Where(root => !root.IsPrefab())
                    .SelectMany(root => root.GetComponentsInChildren<Scope>(includeInactive: true))
                    .ToArray();

                RunInjectionPass
                (
                    collectScopes: () => scopes,
                    noScopesWarning: "Saneject: No scopes found in scene. Nothing to inject.",
                    progressBarTitle: "Saneject: Injection in progress",
                    progressBarMessage: "Injecting all objects in scene",
                    configureGlobalBindings: true,
                    isPrefabInjection: false,
                    buildStatsLabel: _ => $"Scene injection completed [{scene.name}]",
                    addResultToStats: stats,
                    assetItem: sceneItem
                );

                EditorSceneManager.SaveScene(scene);
                numScopesProcessed += scopes.Length;
                Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End scene injection [{scene.name}] <b>↑↑↑</b></color>");
            }

            EditorSceneManager.OpenScene(previousScenePath);

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stats.numScopesProcessed = numScopesProcessed;
            stopwatch.Stop();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(firstSentence: $"Scene batch injection complete | Processed {sceneAssets.Length} scenes");
        }

        /// <summary>
        /// Performs dependency injection across both scenes and prefabs in a single combined pass.
        /// Executes scene injection first, followed by prefab injection, then prints a unified summary log at the end.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="sceneAssets">Array of scene asset items to process, injected in order.</param>
        /// <param name="prefabAssets">Array of prefab asset items to process, injected in order.</param>
        public static void BatchInjectAllScenesAndPrefabs(
            AssetItem[] prefabAssets,
            AssetItem[] sceneAssets)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            string previousScenePath = SceneManager.GetActiveScene().path;
            IEnumerable<string> scenePaths = sceneAssets.Select(scene => scene.Path);
            IEnumerable<string> prefabPaths = prefabAssets.Select(prefab => prefab.Path);

            if (ProxyUtils.TryCreateProxyScriptsForScenesAndPrefabs(scenePaths, prefabPaths))
            {
                EditorSceneManager.OpenScene(previousScenePath);
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            InjectionStats sceneStats = new();
            InjectionStats prefabStats = new();

            BatchInjectScenes(
                sceneAssets: sceneAssets,
                logStats: false,
                stats: sceneStats
            );

            BatchInjectPrefabs(
                prefabAssets: prefabAssets,
                logStats: false,
                stats: prefabStats
            );

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");

            sceneStats.LogStats(
                firstSentence: $"Scene batch injection complete | Processed {sceneAssets.Length} scenes");

            prefabStats.LogStats(firstSentence: $"Prefab batch injection complete | Processed {prefabAssets.Length} prefabs");
        }

        /// <summary>
        /// Executes a single pass of the dependency injection operation based on the provided parameters.
        /// Handles dialog confirmation, scope collection, progress display, and injection processing.
        /// This is an editor-only operation and cannot be performed in Play Mode.
        /// </summary>
        private static void RunInjectionPass(
            Func<Scope[]> collectScopes,
            string noScopesWarning,
            string progressBarTitle,
            string progressBarMessage,
            bool configureGlobalBindings,
            bool isPrefabInjection,
            Func<Scope[], string> buildStatsLabel,
            InjectionStats addResultToStats = null,
            AssetItem assetItem = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Scope[] allScopes = collectScopes?.Invoke() ?? Array.Empty<Scope>();

            if (allScopes.Length <= 0)
            {
                Debug.LogWarning(noScopesWarning);
                stopwatch.Stop();
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar(progressBarTitle, progressBarMessage, 0);
                ScopeExtensions.Initialize(allScopes);
                allScopes.Initialize();

                if (ProxyUtils.TryCreateProxyScripts(allScopes))
                {
                    stopwatch.Stop();
                    allScopes.Dispose();
                    EditorUtility.ClearProgressBar();
                    Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                    return;
                }

                InjectionStats stats = new();

                if (configureGlobalBindings)
                    allScopes.BuildSceneGlobalContainer(stats);

                if (isPrefabInjection)
                {
                    // Prefab pass: inject only the single root scope
                    Scope rootScope = allScopes.FirstOrDefault(s => !s.ParentScope);

                    if (rootScope)
                        InjectFromRootScope(rootScope, stats, isPrefabInjection: true);
                }
                else
                {
                    // Scene / hierarchy pass: inject all root scopes
                    foreach (Scope rootScope in allScopes.Where(s => !s.ParentScope))
                        InjectFromRootScope(rootScope, stats, isPrefabInjection: false);
                }

                Logger.LogUnusedBindings(allScopes, stats);
                stopwatch.Stop();

                if (UserSettings.LogInjectionStats)
                {
                    stats.numScopesProcessed = allScopes.Length;
                    stats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                    stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                    string label = buildStatsLabel?.Invoke(allScopes) ?? "Injection";
                    stats.LogStats(label);
                }

                if (assetItem != null)
                    assetItem.Status = stats.GetInjectionStatus();

                addResultToStats?.AddStats(stats);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogException(e);
            }

            allScopes.Dispose();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Performs recursive injection for all child objects in a hierarchy rooted at a given Scope.
        /// </summary>
        private static void InjectFromRootScope(
            Scope rootScope,
            InjectionStats stats,
            bool isPrefabInjection)
        {
            if (!isPrefabInjection && rootScope.gameObject.IsPrefab())
            {
                if (UserSettings.LogPrefabSkippedDuringSceneInjection)
                    Debug.LogWarning($"Saneject: Skipping scene injection on prefab scope '{rootScope.GetType().Name}' on '{rootScope.gameObject.name}'. The prefab must solve its dependencies using its own scope.", rootScope.gameObject);

                return;
            }

            InjectRecursive(rootScope.transform, rootScope);
            return;

            void InjectRecursive(
                Transform currentTransform,
                Scope currentScope)
            {
                if (!isPrefabInjection && currentTransform.gameObject.IsPrefab())
                {
                    if (UserSettings.LogPrefabSkippedDuringSceneInjection)
                        Debug.LogWarning($"Saneject: Skipping scene injection on prefab '{currentTransform.gameObject.name}'. The prefab must solve its dependencies using its own scope", currentTransform.gameObject);

                    return;
                }

                if (currentTransform.TryGetComponent(out Scope localScope) && localScope != currentScope)
                    currentScope = localScope;

                foreach (MonoBehaviour monoBehaviour in currentTransform.GetComponents<MonoBehaviour>())
                {
                    SerializedObject serializedObject = new(monoBehaviour);

                    // Fields/properties first, persist changes
                    PropertyInjector.InjectSerializedProperties(serializedObject, currentScope, stats);
                    serializedObject.Save();

                    // Then method injection, persist any changes it made
                    MethodInjector.InjectMethods(serializedObject, currentScope, stats);
                    serializedObject.Save();
                }

                foreach (Transform child in currentTransform)
                    InjectRecursive(child, currentScope);
            }
        }
    }
}