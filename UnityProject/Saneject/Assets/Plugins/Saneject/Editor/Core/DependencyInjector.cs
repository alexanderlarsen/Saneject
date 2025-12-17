using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Editor.Core
{
    /// <summary>
    /// Provides editor-only dependency injection for Saneject-enabled scenes and prefabs.
    /// Handles resolving and assigning dependencies for all <see cref="Scope" />s in the current scene or prefab,
    /// based on the binding strategies and filters configured by the user.
    /// </summary>
    public static class DependencyInjector
    {
        // TODO: Change injection algorithm order:
        /* 1. Collect scopes
         * 2. Filter scopes by starting scope context
         * 3. Build parent/child tree
         * 4. Find root scope based on parent-child scope hierarchy
         * 5. Perform DI normally and skip nested scopes with different context than starting scope */

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

            Scope rootScope = startScope.FindRootScope();

            if (!rootScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            InjectionStats stats = new();

            InjectionExecutor.RunInjectionPass
            (
                startScope: startScope,
                createProxyScripts: true,
                $"Single scene hierarchy injection completed [{rootScope.gameObject.name}]",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting all objects in hierarchy",
                globalStats: stats
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
                EditorUtility.DisplayDialog(title: "Saneject", message: "Injection is editor-only. Exit Play Mode to inject.", ok: "Got it");
                return;
            }

            Scope rootScope = startScope ? startScope.FindRootScope() : null;

            if (!rootScope)
            {
                Debug.LogWarning("Saneject: No scopes found in prefab. Nothing to inject.");
                return;
            }

            InjectionStats stats = new();

            InjectionExecutor.RunInjectionPass
            (
                startScope: startScope,
                createProxyScripts: true,
                statsLogPrefix: $"Prefab injection completed [{rootScope.gameObject.name}]",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting prefab dependencies",
                globalStats: stats
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

            HashSet<Scope> rootScopes = scene
                .GetRootGameObjects()
                .SelectMany(scope => scope.GetComponentsInChildren<Scope>(includeInactive: true))
                .Where(scope => !UserSettings.UseContextIsolation || !scope.gameObject.IsPrefab())
                .Select(scope => scope.FindRootScope())
                .ToHashSet();

            if (rootScopes is not { Count: > 0 })
            {
                Debug.LogWarning($"Saneject: No scopes found in scene '{scene.name}'. Nothing to inject.");
                return;
            }

            InjectionStats stats = new();

            foreach (Scope rootScope in rootScopes)
                InjectionExecutor.RunInjectionPass
                (
                    startScope: rootScope,
                    createProxyScripts: true,
                    statsLogPrefix: $"Scene injection completed [{scene.name}]",
                    progressBarTitle: "Saneject: Injection in progress",
                    progressBarMessage: "Injecting all objects in scene",
                    globalStats: stats
                );
        }
    }
}