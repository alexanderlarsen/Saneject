using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Util;
using Plugins.Saneject.Runtime.Bindings;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using Saneject.Plugins.Saneject.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

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
        /// Performs dependency injection for all <see cref="Scope" />s in the currently open scene.
        /// Scans for all configured bindings, resolves them, and assigns references to all eligible fields/properties.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        public static void InjectSceneDependencies()
        {
            RunInjectionPass(
                dialogTitle: "Saneject: Inject Scene Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the scene?",
                askBeforeInject: UserSettings.AskBeforeSceneInjection,
                collectScopes: () => Object
                    .FindObjectsByType<Scope>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(scope => !scope.gameObject.IsPrefab())
                    .ToArray(),
                noScopesWarning: "Saneject: No scopes found in scene. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting all objects in scene",
                configureGlobalBindings: true,
                isPrefabInjection: false,
                buildStatsLabel: _ => "Full scene"
            );
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a hierarchy root in the scene.
        /// Scans for configured bindings starting from the given scope, resolves them recursively up the hierarchy,
        /// and assigns references to all eligible fields/properties.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope to start injection from. All child scopes will be processed.</param>
        public static void InjectSingleHierarchyDependencies(Scope startScope)
        {
            RunInjectionPass(
                dialogTitle: "Saneject: Inject Hierarchy Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the hierarchy?",
                askBeforeInject: UserSettings.AskBeforeHierarchyInjection,
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>()
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
                    return $"Single scene hierarchy [{rootName}]";
                }
            );
        }

        /// <summary>
        /// Performs dependency injection for all <see cref="Scope" />s under a prefab root.
        /// Only used for prefab assets - does not register or inject global dependencies.
        /// This operation is editor-only and cannot be performed in Play Mode.
        /// </summary>
        /// <param name="startScope">The root scope in the prefab to start injection from.</param>
        public static void InjectPrefabDependencies(Scope startScope)
        {
            if (!startScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            RunInjectionPass(
                dialogTitle: "Saneject: Inject Prefab Dependencies",
                confirmationMessage: "Are you sure you want to inject all dependencies in the prefab?",
                askBeforeInject: UserSettings.AskBeforePrefabInjection,
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(),
                noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting prefab dependencies",
                configureGlobalBindings: false,
                isPrefabInjection: true,
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Prefab [{rootName}]";
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
            Func<Scope[], string> buildStatsLabel)
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

                if (UserSettings.ClearLogsOnInjection)
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
                    stats.numInvalidBindings = allScopes.Sum(scope => scope.InvalidBindingsCount);
                    string label = buildStatsLabel?.Invoke(allScopes) ?? "Injection";
                    stats.LogStats(label, allScopes.Length, stopwatch.ElapsedMilliseconds);
                }
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