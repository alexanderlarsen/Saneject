using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Bindings;
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
        /// Performs dependency injection for all <see cref="Scope" />s in the currently active scene.
        /// Scans all non-prefab root objects and their descendants for scopes, processes bindings,
        /// and injects dependencies where applicable.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        public static void InjectCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();

            RunInjectionPass
            (
                dialogTitle: "Saneject: Inject Scene Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the scene?",
                askBeforeInject: false,
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
                clearLogs: false,
                buildStatsLabel: _ => $"Scene injection completed [{scene.name}]"
            );
        }

        /// <summary>
        /// Performs dependency injection across multiple scenes in sequence.
        /// Each scene is opened, processed for <see cref="Scope" />s, injected, saved,
        /// and then the next scene is loaded. Logs progress per scene and aggregates
        /// statistics for all processed scopes.
        /// Restores the originally active scene after completion.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="sceneAssetPaths">An array of scene asset paths to inject, in order of processing.</param>
        /// <param name="canClearLogs"></param>
        /// <param name="stats"></param>
        /// <param name="logStats"></param>
        public static void BatchInjectScenes(
            string[] sceneAssetPaths,
            bool canClearLogs,
            bool logStats,
            InjectionStats stats = null)
        {
            stats ??= new InjectionStats();

            string previousScenePath = SceneManager.GetActiveScene().path;

            if (UserSettings.ClearLogsOnInjection && canClearLogs)
                ConsoleUtils.ClearLog();

            Debug.Log($"<b>──────────  Saneject: Start scene batch injection of {sceneAssetPaths.Length} scene{(sceneAssetPaths.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();
            int numScopesProcessed = 0;

            for (int i = 0; i < sceneAssetPaths.Length; i++)
            {
                Scene scene = EditorSceneManager.OpenScene(sceneAssetPaths[i], OpenSceneMode.Single);
                string logSectionColor = Colors.BatchLogColors[i % Colors.BatchLogColors.Length];

                Debug.Log($"<color={logSectionColor}><b>↓</b> Saneject: Start scene injection [{scene.name}] <b>↓</b></color>");

                Scope[] scopes = scene
                    .GetRootGameObjects()
                    .Where(root => !root.IsPrefab())
                    .SelectMany(root => root.GetComponentsInChildren<Scope>(includeInactive: true))
                    .ToArray();

                RunInjectionPass
                (
                    dialogTitle: "Saneject: Inject Scene Dependencies",
                    confirmationMessage: "Are you sure you want to inject all dependencies in the scene?",
                    askBeforeInject: false,
                    collectScopes: () => scopes,
                    noScopesWarning: "Saneject: No scopes found in scene. Nothing to inject.",
                    progressBarTitle: "Saneject: Injection in progress",
                    progressBarMessage: "Injecting all objects in scene",
                    configureGlobalBindings: true,
                    isPrefabInjection: false,
                    clearLogs: false,
                    buildStatsLabel: _ => $"Scene injection completed [{scene.name}]",
                    addResultToStats: stats
                );

                EditorSceneManager.SaveScene(scene);
                numScopesProcessed += scopes.Length;

                Debug.Log($"<color={logSectionColor}><b>↑</b> Saneject: End scene injection [{scene.name}] <b>↑</b></color>");
            }

            EditorSceneManager.OpenScene(previousScenePath);

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stats.numScopesProcessed = numScopesProcessed;
            stopwatch.Stop();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(firstSentence: $"Scene batch injection complete | Processed {sceneAssetPaths.Length} scenes");
        }

        public static void BatchInjectPrefabs(
            string[] prefabAssetPaths,
            bool canClearLogs,
            bool logStats,
            InjectionStats stats = null)
        {
            stats ??= new InjectionStats();

            if (UserSettings.ClearLogsOnInjection && canClearLogs)
                ConsoleUtils.ClearLog();

            Debug.Log($"<b>──────────  Saneject: Starting prefab batch injection of {prefabAssetPaths.Length} prefab{(prefabAssetPaths.Length != 1 ? "s" : "")}  ──────────</b>");

            Stopwatch stopwatch = Stopwatch.StartNew();
            int numScopesProcessed = 0;

            for (int i = 0; i < prefabAssetPaths.Length; i++)
            {
                string path = prefabAssetPaths[i];
                string prefabName = Path.GetFileNameWithoutExtension(path);
                string logSectionColor = Colors.BatchLogColors[i % Colors.BatchLogColors.Length];

                Debug.Log($"<color={logSectionColor}><b>↓</b> Saneject: Start prefab injection [{prefabName}] <b>↓</b></color>");

                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (!prefabAsset)
                {
                    Debug.LogWarning($"Saneject: Failed to load prefab asset at '{path}'. Skipping.");

                    Debug.Log($"<color={logSectionColor}><b>↑</b> Saneject: End prefab injection [{prefabName}] <b>↑</b></color>");
                    continue;
                }

                Scope[] scopes = prefabAsset.GetComponentsInChildren<Scope>(true);

                if (scopes.Length == 0)
                {
                    Debug.LogWarning($"Saneject: No scopes found in prefab '{prefabName}'. Nothing to inject.");

                    Debug.Log($"<color={logSectionColor}><b>↑</b> Saneject: End prefab injection [{prefabName}] <b>↑</b></color>");
                    continue;
                }

                RunInjectionPass(
                    dialogTitle: "Saneject: Inject Prefab Dependencies",
                    confirmationMessage: "Are you sure you want to inject all dependencies in the prefab?",
                    askBeforeInject: false,
                    collectScopes: () => scopes,
                    noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                    progressBarTitle: "Saneject: Injection in progress",
                    progressBarMessage: $"Injecting prefab dependencies ({prefabName})",
                    configureGlobalBindings: false,
                    isPrefabInjection: true,
                    clearLogs: false,
                    buildStatsLabel: _ => $"Prefab injection completed [{prefabName}]",
                    addResultToStats: stats
                );

                // Save serialized changes to the prefab asset
                EditorUtility.SetDirty(prefabAsset);
                AssetDatabase.SaveAssets();

                numScopesProcessed += scopes.Length;

                Debug.Log($"<color={logSectionColor}><b>↑</b> Saneject: End prefab injection [{prefabName}] <b>↑</b></color>");
            }

            stats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            stats.numScopesProcessed = numScopesProcessed;
            stopwatch.Stop();

            if (!logStats)
                return;

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");
            stats.LogStats(firstSentence: $"Prefab batch injection complete | Processed {prefabAssetPaths.Length} prefabs");
        }

        public static void BatchInjectAllScenesAndPrefabs(
            string[] sceneAssetPaths,
            string[] prefabAssetPaths)
        {
            InjectionStats sceneStats = new();
            InjectionStats prefabStats = new();

            BatchInjectScenes(
                sceneAssetPaths: sceneAssetPaths,
                canClearLogs: true,
                logStats: false,
                stats: sceneStats
            );

            BatchInjectPrefabs(
                prefabAssetPaths: prefabAssetPaths,
                canClearLogs: false,
                logStats: false,
                stats: prefabStats
            );

            Debug.Log("<b>──────────  Saneject: Batch injection summary  ──────────</b>");

            sceneStats.LogStats(
                firstSentence: $"Scene batch injection complete | Processed {sceneAssetPaths.Length} scenes");

            prefabStats.LogStats(firstSentence: $"Prefab batch injection complete | Processed {prefabAssetPaths.Length} prefabs");
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a hierarchy root in the scene.
        /// Scans for configured bindings starting from the given scope, resolves them recursively up the hierarchy,
        /// and assigns references to all eligible fields/properties.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope to start injection from. All child scopes will be processed.</param>
        public static void InjectSingleHierarchy(Scope startScope)
        {
            RunInjectionPass
            (
                dialogTitle: "Saneject: Inject Hierarchy Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the hierarchy?",
                askBeforeInject: UserSettings.AskBeforeHierarchyInjection,
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
                clearLogs: UserSettings.ClearLogsOnInjection,
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
            if (!startScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            RunInjectionPass
            (
                dialogTitle: "Saneject: Inject Prefab Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the prefab?",
                askBeforeInject: UserSettings.AskBeforePrefabInjection,
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(includeInactive: true),
                noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting prefab dependencies",
                configureGlobalBindings: false,
                isPrefabInjection: true,
                clearLogs: UserSettings.ClearLogsOnInjection,
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Prefab injection completed [{rootName}]";
                }
            );
        }

        /// <summary>
        /// Executes a single pass of the dependency injection operation based on the provided parameters.
        /// Handles dialog confirmation, scope collection, progress display, and injection processing.
        /// This is an editor-only operation and cannot be performed in Play Mode.
        /// </summary>
        private static void RunInjectionPass(
            string dialogTitle,
            string confirmationMessage,
            bool askBeforeInject,
            Func<Scope[]> collectScopes,
            string noScopesWarning,
            string progressBarTitle,
            string progressBarMessage,
            bool configureGlobalBindings,
            bool isPrefabInjection,
            bool clearLogs,
            Func<Scope[], string> buildStatsLabel,
            InjectionStats addResultToStats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog(dialogTitle, "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            // Application.isBatchMode is true when run from CI tests and CI test runner can't confirm the dialog so we need to skip it in that case.
            if (!Application.isBatchMode)
            {
                if (askBeforeInject && !EditorUtility.DisplayDialog(dialogTitle, confirmationMessage, "Yes", "Cancel"))
                    return;

                if (clearLogs)
                    ConsoleUtils.ClearLog();
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

                IEnumerable<Binding> proxyBindings = allScopes
                    .SelectMany(scope => scope.GetProxyBindings());

                proxyBindings.CreateMissingProxyStubs(out bool isProxyCreationPending);

                if (isProxyCreationPending)
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