using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.BatchInjection;
using Plugins.Saneject.Experimental.Editor.Data.Context;
using Plugins.Saneject.Experimental.Editor.Data.Graph;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Data.Injection;
using Plugins.Saneject.Experimental.Editor.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Pipeline
{
    public static class InjectionRunner
    {
        public static void Run(
            IEnumerable<Object> startObjects,
            ContextWalkFilter contextWalkFilter)
        {
            if (Application.isPlaying)
            {
                DialogUtility.InjectionPipeline.Display_EditorOnlyInjection();
                return;
            }

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
            if (Application.isPlaying)
            {
                DialogUtility.InjectionPipeline.Display_EditorOnlyInjection();
                return;
            }

            HashSet<SceneBatchItem> sceneBatchItems = batchItems
                .OfType<SceneBatchItem>()
                .ToHashSet();

            HashSet<PrefabBatchItem> prefabBatchItems = batchItems
                .OfType<PrefabBatchItem>()
                .ToHashSet();

            Logger.TryClearLog();

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Logger.LogInjectionCancelledByUser();
                return;
            }

            Stopwatch sceneStopwatch = Stopwatch.StartNew();
            InjectionResults sceneResults = RunSceneBatch(sceneBatchItems);
            sceneStopwatch.Stop();

            Stopwatch prefabStopwatch = Stopwatch.StartNew();
            InjectionResults prefabResults = RunPrefabBatch(prefabBatchItems);
            prefabStopwatch.Stop();

            Logger.LogBatchSummary
            (
                sceneBatchItems.Count,
                prefabBatchItems.Count,
                sceneResults,
                prefabResults,
                sceneStopwatch.ElapsedMilliseconds,
                prefabStopwatch.ElapsedMilliseconds
            );
        }

        private static InjectionResults RunSceneBatch(IEnumerable<SceneBatchItem> batchItems)
        {
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

            return combinedResults;
        }

        private static InjectionResults RunPrefabBatch(IEnumerable<PrefabBatchItem> batchItems)
        {
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
            return combinedResults;
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