using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Legacy.Editor.Extensions;
using Plugins.Saneject.Legacy.Runtime.Extensions;
using Plugins.Saneject.Legacy.Runtime.Scopes;
using Plugins.Saneject.Legacy.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Legacy.Editor.Core
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
            InjectionExecutor.RunInjectionPassSingle
            (
                startScope: startScope,
                createProxyScripts: true,
                logStats: true,
                statsLogPrefix: "Single scene hierarchy injection completed",
                progressBarMessage: "Injecting all objects in hierarchy"
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
            InjectionExecutor.RunInjectionPassSingle
            (
                startScope: startScope,
                createProxyScripts: true,
                logStats: true,
                statsLogPrefix: "Prefab injection completed",
                progressBarMessage: "Injecting prefab dependencies"
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

            InjectionExecutor.RunInjectionPassMultiple
            (
                rootScopes,
                createProxyScripts: true,
                logStats: true,
                statsLogPrefix: $"Full scene injection completed [{scene.name}]",
                progressBarMessage: "Injecting all objects in scene"
            );
        }
    }
}