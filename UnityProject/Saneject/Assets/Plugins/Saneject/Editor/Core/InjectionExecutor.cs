using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Editor.BatchInjection.Data;
using Plugins.Saneject.Editor.BatchInjection.Utilities;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Plugins.Saneject.Editor.Core
{
    /// <summary>
    /// Central executor for Saneject’s editor-time dependency injection pipeline.
    /// Provides a unified entry point for performing scoped injection passes across prefabs,
    /// scenes, or arbitrary object hierarchies. Handles scope initialization, proxy creation,
    /// dependency resolution, serialized field and method injection, progress display, and statistics collection.
    /// <para>
    /// This class is editor-only and designed for use by higher-level tools such as
    /// <see cref="DependencyInjector" /> and <see cref="Plugins.Saneject.Editor.BatchInjection.Core.BatchInjector" />.
    /// </para>
    /// </summary>
    public static class InjectionExecutor
    {
        /// <summary>
        /// Executes a single pass of the dependency injection operation based on the provided parameters.
        /// Handles dialog confirmation, scope collection, progress display, and injection processing.
        /// This is an editor-only operation and cannot be performed in Play Mode.
        /// </summary>
        public static void RunInjectionPassSingle(
            Scope startScope,
            bool createProxyScripts,
            string statsLogPrefix,
            bool logStats,
            string progressBarMessage = null,
            AssetData assetData = null,
            InjectionStats globalStats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            Scope rootScope = startScope ? startScope.FindRootScope() : null;

            if (!rootScope)
            {
                Debug.LogWarning("Saneject: No scopes found in this context. Nothing to inject.");
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            Scope[] allScopes = rootScope
                .GetComponentsInChildren<Scope>(includeInactive: true)
                .Where(scope => ContextFilter.AreSameContext(scope, rootScope))
                .ToArray();

            try
            {
                if (progressBarMessage != null)
                    EditorUtility.DisplayProgressBar("Saneject: Injection in progress", progressBarMessage, 0);

                foreach (Scope scope in allScopes)
                {
                    scope.SetParentScope();
                    scope.ConfigureBindings();
                    scope.ValidateBindings();
                }

                if (createProxyScripts && ProxyUtils.TryCreateProxyScripts(allScopes))
                {
                    stopwatch.Stop();
                    allScopes.DisposeAll();
                    EditorUtility.ClearProgressBar();
                    Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                    return;
                }

                InjectionStats localStats = new();

                if (!rootScope.gameObject.IsPrefab())
                    allScopes.BuildSceneGlobalContainer(localStats);

                InjectRecursive
                (
                    rootScope: rootScope,
                    currentTransform: rootScope.transform,
                    currentScope: rootScope,
                    stats: localStats
                );

                Logger.LogUnusedBindings(allScopes, localStats);
                stopwatch.Stop();

                if (assetData != null)
                    assetData.Status = InjectionStatusUtils.GetInjectionStatusFromStats(localStats);

                localStats.numScopesProcessed = allScopes.Length;
                localStats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                localStats.elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                globalStats?.AddStats(localStats);

                if (logStats && UserSettings.LogInjectionSummary)
                    localStats.LogStats(prefix: $"{statsLogPrefix} [{rootScope.gameObject.name}]");
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogException(e);
            }

            allScopes.DisposeAll();

            if (progressBarMessage != null)
                EditorUtility.ClearProgressBar();
        }

        public static void RunInjectionPassMultiple(
            IEnumerable<Scope> startScopes,
            bool createProxyScripts,
            string statsLogPrefix,
            bool logStats,
            string progressBarMessage = null,
            AssetData assetData = null,
            InjectionStats globalStats = null)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Saneject", "Injection is editor-only. Exit Play Mode to inject.", "Got it");
                return;
            }

            Scope[] scopesArray = startScopes.ToArray();

            if (scopesArray is not { Length: > 0 })
            {
                Debug.LogWarning("Saneject: No scopes found in this context. Nothing to inject.");
                return;
            }

            globalStats ??= new InjectionStats();

            foreach (Scope startScope in scopesArray)
                RunInjectionPassSingle
                (
                    startScope,
                    createProxyScripts,
                    statsLogPrefix,
                    logStats: false,
                    progressBarMessage,
                    assetData,
                    globalStats);

            if (logStats && UserSettings.LogInjectionSummary)
                globalStats.LogStats(prefix: $"{statsLogPrefix}");
        }

        private static void InjectRecursive(
            Scope rootScope,
            Transform currentTransform,
            Scope currentScope,
            InjectionStats stats)
        {
            if (currentTransform.TryGetComponent(out Scope localScope) && localScope != currentScope)
                currentScope = localScope;

            if (!ContextFilter.AreSameContext(rootScope, currentTransform))
            {
                if (UserSettings.LogDifferentContextSkipping)
                    Debug.LogWarning($"Saneject: Skipping injection on '{currentTransform.gameObject.name}' due to different context.",
                        currentTransform.gameObject);

                foreach (Transform child in currentTransform)
                    InjectRecursive
                    (
                        rootScope: rootScope,
                        currentTransform: child,
                        currentScope: currentScope,
                        stats: stats
                    );

                return;
            }

            foreach (MonoBehaviour monoBehaviour in currentTransform.GetComponents<MonoBehaviour>())
            {
                SerializedObject serializedObject = new(monoBehaviour);

                // Fields/properties first, persist changes
                PropertyInjector.InjectSerializedProperties(serializedObject,
                    currentScope,
                    stats);

                serializedObject.Save();

                // Then method injection, persist any changes it made
                MethodInjector.InjectMethods(serializedObject,
                    currentScope,
                    stats);

                serializedObject.Save();
            }

            foreach (Transform child in currentTransform)
                InjectRecursive
                (
                    rootScope: rootScope,
                    currentTransform: child,
                    currentScope: currentScope,
                    stats: stats
                );
        }
    }
}