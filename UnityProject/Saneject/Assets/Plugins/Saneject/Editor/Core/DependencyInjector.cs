using System.Linq;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Extensions;
using Plugins.Saneject.Runtime.Scopes;
using UnityEditor;
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

            InjectionExecutor.RunInjectionPass
            (
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(includeInactive: true)
                    .Where(scope => !scope.gameObject.IsPrefab())
                    .ToArray(),
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Single scene hierarchy injection completed [{rootName}]";
                },
                isPrefabInjection: false,
                createProxyScripts: true,
                noScopesWarning: "Saneject: No scopes found in hierarchy. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
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

            InjectionExecutor.RunInjectionPass
            (
                collectScopes: () => startScope
                    .FindRootScope()
                    .GetComponentsInChildren<Scope>(includeInactive: true),
                buildStatsLabel: scopes =>
                {
                    Scope root = scopes.FirstOrDefault(s => !s.ParentScope);
                    string rootName = root ? root.gameObject.name : "Unknown";
                    return $"Prefab injection completed [{rootName}]";
                },
                isPrefabInjection: true,
                createProxyScripts: true,
                noScopesWarning: "Saneject: No scopes found in prefab. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
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

            InjectionExecutor.RunInjectionPass
            (
                collectScopes: () => scene
                    .GetRootGameObjects()
                    .Where(root => !root.IsPrefab())
                    .SelectMany(root => root.GetComponentsInChildren<Scope>(includeInactive: true))
                    .ToArray(),
                buildStatsLabel: _ => $"Scene injection completed [{scene.name}]",
                isPrefabInjection: false,
                createProxyScripts: true,
                noScopesWarning: "Saneject: No scopes found in scene. Nothing to inject.",
                progressBarTitle: "Saneject: Injection in progress",
                progressBarMessage: "Injecting all objects in scene"
            );
        }
    }
}