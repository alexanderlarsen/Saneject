using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionRunner
    {
        public static void Run(
            IEnumerable<GameObject> startGameObjects,
            WalkFilter walkFilter)
        {
            Transform[] startTransforms = startGameObjects
                .Select(x => x.transform)
                .ToArray();

            Run(startTransforms, walkFilter);
        }

        public static void Run(
            Transform[] startTransforms,
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();
            InjectionContext context = new(walkFilter, startTransforms);
            BindingValidator.ValidateBindings(context);
            Resolver.Resolve(context);
            Injector.InjectDependencies(context);
            InjectionContextJsonProjector.SaveToDisk(context); // TODO: Move out of this method
            InjectionResults results = context.GetResults();
            Logger.LogResults(results);
            Logger.LogSummary(results);
        }

        public static void RunBatch(
            string[] scenePaths,
            string[] prefabPaths,
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();

            // TODO: This most likely won't work because the objects used to create the context/graph are destroyed when the scene is unloaded. Probably best to just gather all contexts in a single hashset, discard them after proxy generation and generate them again in each scene/prefab loop. Maybe build a cheap pre-pass graph that only finds and validates bindings, instead of the entire thing - or not even a graph but a binding scan across scenes and prefabs. Remember to use the walk filter to find the scopes.

            Dictionary<string, InjectionContext> sceneContextMap = new();
            Dictionary<string, InjectionContext> prefabContextMap = new();

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath);

                InjectionContext context = new(
                    walkFilter,
                    startTransforms: scene
                        .GetRootGameObjects()
                        .Select(x => x.transform)
                        .ToArray()
                );

                BindingValidator.ValidateBindings(context); // TODO: To enable binding scan, I need to pass bindings directly here.
                sceneContextMap[scenePath] = context;
            }

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                InjectionContext context = new(
                    walkFilter,
                    startTransforms: prefabAsset.transform
                );

                BindingValidator.ValidateBindings(context); // TODO: ... and here
                prefabContextMap[prefabPath] = context;
            }

            InjectionResults sceneResults = new();
            InjectionResults prefabResults = new();

            foreach (string scenePath in scenePaths)
            {
                // Open scene and pass all its root GameObjects to RunSingle
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                InjectionContext context = sceneContextMap[scenePath];
                Resolver.Resolve(context);
                Injector.InjectDependencies(context);
                InjectionContextJsonProjector.SaveToDisk(context); // TODO: Move out of this method
                InjectionResults results = context.GetResults();
                sceneResults.AddToResults(results);
                Logger.LogResults(results);
            }

            foreach (string prefabPath in prefabPaths)
            {
                // Open prefab and pass its root GameObject to RunSingle
                InjectionContext context = prefabContextMap[prefabPath];
                Resolver.Resolve(context);
                Injector.InjectDependencies(context);
                InjectionContextJsonProjector.SaveToDisk(context); // TODO: Move out of this method
                InjectionResults results = context.GetResults();
                prefabResults.AddToResults(results);
                Logger.LogResults(results);
            }

            Logger.LogSummary(sceneResults);
            Logger.LogSummary(prefabResults);
        }
    }
}