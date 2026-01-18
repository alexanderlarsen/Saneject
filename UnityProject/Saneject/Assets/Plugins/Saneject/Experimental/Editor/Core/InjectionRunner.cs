using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// ReSharper disable LoopCanBePartlyConvertedToQuery

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
            string[] sceneGuids,
            string[] prefabGuids,
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();

            InjectionResults sceneResults = new();
            InjectionResults prefabResults = new();

            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                Transform[] startTransforms = EditorSceneManager
                    .OpenScene(path, OpenSceneMode.Single)
                    .GetRootGameObjects()
                    .Select(x => x.transform)
                    .ToArray();

                InjectionContext context = new(walkFilter, startTransforms);
                BindingValidator.ValidateBindings(context);
                Resolver.Resolve(context);
                Injector.InjectDependencies(context);
                InjectionResults results = context.GetResults();
                sceneResults.AddToResults(results);
                Logger.LogResults(results);
            }

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                Transform startTransform = AssetDatabase
                    .LoadAssetAtPath<GameObject>(path)
                    .transform
                    .root;

                InjectionContext context = new(walkFilter, startTransform);
                BindingValidator.ValidateBindings(context);
                Resolver.Resolve(context);
                Injector.InjectDependencies(context);
                InjectionResults results = context.GetResults();
                prefabResults.AddToResults(results);
                Logger.LogResults(results);
            }

            Logger.LogSummary(sceneResults);
            Logger.LogSummary(prefabResults);
        }
    }
}