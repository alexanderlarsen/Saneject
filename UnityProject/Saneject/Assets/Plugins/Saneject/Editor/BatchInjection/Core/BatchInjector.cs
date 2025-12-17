using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Enums;
using Plugins.Saneject.Editor.Core;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Plugins.Saneject.Editor.BatchInjection.Core
{
    /// <summary>
    /// Provides static methods for batch injection of dependencies into Unity assets such as prefabs and scenes.
    /// This class is specifically designed to work with Saneject's injection system.
    /// </summary>
    public static class BatchInjector
    {
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
            AssetData[] prefabAssets,
            bool logStats,
            InjectionStats stats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            EditorUtility.DisplayProgressBar("Saneject: Batch injection in progress", "Starting batch prefab injection", 0);
            prefabAssets = prefabAssets.Where(s => s.Asset != null).ToArray();

            foreach (AssetData asset in prefabAssets)
                asset.Status = InjectionStatus.Unknown;

            if (ProxyUtils.TryCreateProxyScriptsForPrefabs(prefabAssets.Select(prefab => prefab.Path)))
            {
                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            stats ??= new InjectionStats();

            Debug.Log($"<b>──────────  Saneject: Starting prefab batch injection of {prefabAssets.Length} prefab{(prefabAssets.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < prefabAssets.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Saneject: Batch injection in progress", $"Injecting all objects in prefab '{prefabAssets[i].Name}'", i / (float)prefabAssets.Length);
                AssetData prefabData = prefabAssets[i];
                string path = prefabData.Path;
                string prefabName = Path.GetFileNameWithoutExtension(path);
                string logSectionColor = EditorColors.BatchLogColors[i % EditorColors.BatchLogColors.Length];

                prefabData.Status = InjectionStatus.Unknown;

                Debug.Log($"<color={logSectionColor}><b>↓↓↓</b> Saneject: Start prefab injection [{prefabName}] <b>↓↓↓</b></color>", prefabData.Asset);

                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (!prefabAsset)
                    Debug.LogWarning($"Saneject: Failed to load prefab asset at '{path}'. Skipping.");

                Scope rootScope = prefabAsset ? prefabAsset.GetComponentInChildren<Scope>() : null;

                if (rootScope)
                {
                    InjectionExecutor.RunInjectionPass
                    (
                        startScope: rootScope,
                        createProxyScripts: false,
                        statsLogPrefix: $"Prefab injection completed [{rootScope.gameObject.name}]",
                        globalStats: stats
                    );

                    EditorUtility.SetDirty(prefabAsset);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                }

                Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End prefab injection [{prefabName}] <b>↑↑↑</b></color>", prefabData.Asset);
            }

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            EditorUtility.ClearProgressBar();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(prefix: $"Prefab batch injection complete | Processed {prefabAssets.Length} prefabs");
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
            AssetData[] sceneAssets,
            bool logStats,
            InjectionStats stats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            EditorUtility.DisplayProgressBar("Saneject: Batch injection in progress", "Starting batch scene injection", 0);
            sceneAssets = sceneAssets.Where(s => s.Asset != null).ToArray();

            foreach (AssetData asset in sceneAssets)
                asset.Status = InjectionStatus.Unknown;

            string previousScenePath = SceneManager.GetActiveScene().path;

            if (ProxyUtils.TryCreateProxyScriptsForScenes(sceneAssets.Select(scene => scene.Path)))
            {
                if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != ".")
                    EditorSceneManager.OpenScene(previousScenePath);

                EditorUtility.ClearProgressBar();
                Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                return;
            }

            stats ??= new InjectionStats();

            Debug.Log($"<b>──────────  Saneject: Start scene batch injection of {sceneAssets.Length} scene{(sceneAssets.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < sceneAssets.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Saneject: Batch injection in progress", $"Injecting all objects in scene '{sceneAssets[i].Name}'", i / (float)sceneAssets.Length);
                AssetData sceneData = sceneAssets[i];
                Scene scene = EditorSceneManager.OpenScene(sceneData.Path, OpenSceneMode.Single);
                string logSectionColor = EditorColors.BatchLogColors[i % EditorColors.BatchLogColors.Length];
                sceneData.Status = InjectionStatus.Unknown;

                Debug.Log($"<color={logSectionColor}><b>↓↓↓</b> Saneject: Start scene injection [{scene.name}] <b>↓↓↓</b></color>", sceneData.Asset);

                HashSet<Scope> rootScopes = scene
                    .GetRootGameObjects()
                    .SelectMany(scope => scope.GetComponentsInChildren<Scope>(includeInactive: true))
                    .Where(scope => !UserSettings.UseContextIsolation || !scope.gameObject.IsPrefab())
                    .Select(scope => scope.FindRootScope())
                    .ToHashSet();

                if (rootScopes is { Count: > 0 })
                {
                    foreach (Scope rootScope in rootScopes)
                        InjectionExecutor.RunInjectionPass
                        (
                            startScope: rootScope,
                            statsLogPrefix: $"Scene injection completed [{scene.name}]",
                            createProxyScripts: false,
                            globalStats: stats
                        );

                    EditorSceneManager.SaveScene(scene);
                }
                else
                {
                    Debug.LogWarning($"Saneject: No scopes found in scene '{scene.name}'. Nothing to inject.");
                }

                Debug.Log($"<color={logSectionColor}><b>↑↑↑</b> Saneject: End scene injection [{scene.name}] <b>↑↑↑</b></color>", sceneData.Asset);
            }

            if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != ".")
                EditorSceneManager.OpenScene(previousScenePath);

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            EditorUtility.ClearProgressBar();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(prefix: $"Scene batch injection complete | Processed {sceneAssets.Length} scenes");
        }

        /// <summary>
        /// Performs dependency injection across both scenes and prefabs in a single combined pass.
        /// Executes scene injection first, followed by prefab injection, then prints a unified summary log at the end.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="sceneAssets">Array of scene asset items to process, injected in order.</param>
        /// <param name="prefabAssets">Array of prefab asset items to process, injected in order.</param>
        public static void BatchInjectAllScenesAndPrefabs(
            AssetData[] prefabAssets,
            AssetData[] sceneAssets)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            EditorUtility.DisplayProgressBar("Saneject: Batch injection in progress", "Starting batch scene and prefab injection", 0);

            foreach (AssetData asset in sceneAssets.Union(prefabAssets))
                asset.Status = InjectionStatus.Unknown;

            string previousScenePath = SceneManager.GetActiveScene().path;
            IEnumerable<string> scenePaths = sceneAssets.Select(scene => scene.Path);
            IEnumerable<string> prefabPaths = prefabAssets.Select(prefab => prefab.Path);

            if (ProxyUtils.TryCreateProxyScriptsForScenesAndPrefabs(scenePaths, prefabPaths))
            {
                if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != ".")
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
            sceneStats.LogStats(prefix: $"Scene batch injection complete | Processed {sceneAssets.Length} scenes");
            prefabStats.LogStats(prefix: $"Prefab batch injection complete | Processed {prefabAssets.Length} prefabs");
            EditorUtility.ClearProgressBar();
        }
    }
}