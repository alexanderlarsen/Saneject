using System;
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
    /// <see cref="DependencyInjector"/> and <see cref="Plugins.Saneject.Editor.BatchInjection.Core.BatchInjector"/>.
    /// </para>
    /// </summary>
    public static class InjectionExecutor
    {
        /// <summary>
        /// Executes a single pass of the dependency injection operation based on the provided parameters.
        /// Handles dialog confirmation, scope collection, progress display, and injection processing.
        /// This is an editor-only operation and cannot be performed in Play Mode.
        /// </summary>
        public static void RunInjectionPass(
            Func<Scope[]> collectScopes,
            Func<Scope[], string> buildStatsLabel,
            bool isPrefabInjection,
            bool createProxyScripts,
            string noScopesWarning,
            string progressBarTitle = null,
            string progressBarMessage = null,
            AssetData assetData = null,
            InjectionStats addResultToStats = null)
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
                if (progressBarTitle != null && progressBarMessage != null)
                    EditorUtility.DisplayProgressBar(progressBarTitle, progressBarMessage, 0);

                allScopes.InitializeScopes();

                if (createProxyScripts && ProxyUtils.TryCreateProxyScripts(allScopes))
                {
                    stopwatch.Stop();
                    allScopes.Dispose();
                    EditorUtility.ClearProgressBar();
                    Debug.LogWarning("Saneject: Injection aborted due to proxy script creation.");
                    return;
                }

                InjectionStats stats = new();

                if (!isPrefabInjection)
                    allScopes.BuildSceneGlobalContainer(stats);

                foreach (Scope rootScope in allScopes.Where(scope => !scope.ParentScope))
                {
                    if (!isPrefabInjection && rootScope.gameObject.IsPrefab())
                    {
                        if (UserSettings.LogPrefabSkippedDuringSceneInjection)
                            Debug.LogWarning($"Saneject: Skipping scene injection on prefab scope '{rootScope.GetType().Name}' on '{rootScope.gameObject.name}'. The prefab must solve its dependencies using its own scope.", rootScope.gameObject);

                        continue;
                    }

                    InjectRecursive(rootScope.transform, rootScope, stats, isPrefabInjection);
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

                if (assetData != null)
                    assetData.Status = InjectionStatusUtils.GetInjectionStatusFromStats(stats);

                addResultToStats?.AddStats(stats);
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogException(e);
            }

            allScopes.Dispose();

            if (progressBarTitle != null && progressBarMessage != null)
                EditorUtility.ClearProgressBar();
        }

        private static void InjectRecursive(
            Transform currentTransform,
            Scope currentScope,
            InjectionStats stats,
            bool isPrefabInjection)
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
                InjectRecursive(child, currentScope, stats, isPrefabInjection);
        }
    }
}