using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            Logger.LogSummary
            (
                prefix: "Injection complete",
                results,
                stopwatch.ElapsedMilliseconds
            );
        }

        public static void RunBatch(IReadOnlyCollection<BatchItem> batchItems)
        {
            HashSet<SceneBatchItem> sceneBatchItems = batchItems
                .OfType<SceneBatchItem>()
                .ToHashSet();

            HashSet<PrefabBatchItem> prefabBatchItems = batchItems
                .OfType<PrefabBatchItem>()
                .ToHashSet();

            Logger.TryClearLog();

            if (sceneBatchItems.Count == 0 && prefabBatchItems.Count == 0)
            {
                Logger.LogNothingToInject();
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Logger.LogInjectionCancelledByUser();
                return;
            }

            if (sceneBatchItems.Count > 0)
                RunSceneBatch(sceneBatchItems);

            if (prefabBatchItems.Count > 0)
                RunPrefabBatch(prefabBatchItems);
        }

        private static void RunSceneBatch(IEnumerable<SceneBatchItem> batchItems)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            InjectionResults combinedResults = new();

            string initialScenePath = SceneManager.GetActiveScene().path;

            foreach (SceneBatchItem item in batchItems)
            {
                Scene scene = EditorSceneManager.OpenScene(item.Path, OpenSceneMode.Single);

                IEnumerable<Transform> startObjects = scene
                    .GetRootGameObjects()
                    .Select(x => x.transform);

                InjectionResults results = RunContext(startObjects, item.ContextWalkFilter);
                EditorSceneManager.SaveScene(scene);
                combinedResults.AddToResults(results);
            }

            if (!string.IsNullOrEmpty(initialScenePath))
                EditorSceneManager.OpenScene(initialScenePath, OpenSceneMode.Single);

            stopwatch.Stop();

            Logger.LogSummary
            (
                prefix: "Scene batch injection complete",
                combinedResults,
                stopwatch.ElapsedMilliseconds
            );
        }

        private static void RunPrefabBatch(IEnumerable<PrefabBatchItem> batchItems)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            InjectionResults combinedResults = new();

            foreach (PrefabBatchItem item in batchItems)
            {
                Transform[] startObjects =
                {
                    AssetDatabase
                        .LoadAssetAtPath<GameObject>(item.Path)
                        .transform
                        .root
                };

                InjectionResults results = RunContext(startObjects, ContextWalkFilter.PrefabAsset);
                combinedResults.AddToResults(results);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            stopwatch.Stop();

            Logger.LogSummary
            (
                prefix: "Prefab batch injection complete",
                combinedResults,
                stopwatch.ElapsedMilliseconds
            );
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
            /*InjectionContextJsonProjector.SaveToDisk(context, injectionGraph);*/ // TODO: Make batch friendly
            return results;
        }
    }
}