using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionRunner
    {
        public static void Run(
            IEnumerable<Object> startObjects,
            ContextWalkFilter contextWalkFilter)
        {
            Logger.TryClearLog();
            Stopwatch stopwatch = Stopwatch.StartNew();
            InjectionResults results = RunContext(startObjects, contextWalkFilter);
            stopwatch.Stop();
            Logger.LogSummary(results, stopwatch.ElapsedMilliseconds);
        }

        public static void RunBatch(
            string[] sceneGuids,
            string[] prefabGuids,
            ContextWalkFilter contextWalkFilter)
        {
            Logger.TryClearLog();
            Stopwatch sceneStopwatch = new();
            Stopwatch prefabStopwatch = new();
            InjectionResults sceneResults = new();
            InjectionResults prefabResults = new();
            sceneStopwatch.Start();

            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                IEnumerable<Transform> startObjects = EditorSceneManager
                    .OpenScene(path, OpenSceneMode.Single)
                    .GetRootGameObjects()
                    .Select(x => x.transform);

                InjectionResults results = RunContext(startObjects, contextWalkFilter);
                sceneResults.AddToResults(results);
            }

            sceneStopwatch.Stop();
            prefabStopwatch.Start();

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                Transform[] startObjects =
                {
                    AssetDatabase
                        .LoadAssetAtPath<GameObject>(path)
                        .transform
                        .root
                };

                InjectionResults results = RunContext(startObjects, contextWalkFilter);
                prefabResults.AddToResults(results);
            }

            prefabStopwatch.Stop();
            Logger.LogSummary(sceneResults, sceneStopwatch.ElapsedMilliseconds);
            Logger.LogSummary(prefabResults, prefabStopwatch.ElapsedMilliseconds);
        }

        private static InjectionResults RunContext(
            IEnumerable<Object> startObjects,
            ContextWalkFilter contextWalkFilter)
        {
            Transform[] startTransforms = startObjects switch
            {
                Transform[] t => t,
                IEnumerable<Transform> t => t.ToArray(),
                IEnumerable<GameObject> g => g.Select(x => x.transform).ToArray(),
                IEnumerable<Component> c => c.Select(x => x.transform).ToArray(),
                _ => throw new Exception("Unsupported start object type.")
            };

            InjectionGraph injectionGraph = new(startTransforms);
            IReadOnlyCollection<TransformNode> activeTransformNodes = GraphFilter.ApplyWalkFilter(injectionGraph, startTransforms, contextWalkFilter);
            InjectionContext context = new(activeTransformNodes);
            BindingValidator.ValidateBindings(context);
            Resolver.Resolve(context);
            Injector.InjectDependencies(context);
            InjectionResults results = context.GetResults();
            Logger.LogResults(results);
            InjectionContextJsonProjector.SaveToDisk(context, injectionGraph); // TODO: Make batch friendly
            return results;
        }
    }
}