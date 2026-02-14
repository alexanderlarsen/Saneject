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
            InjectionProgressTracker progressTracker = new(totalSegments: 10);
            progressTracker.SetTitle("Injecting");

            RunContext
            (
                startObjects,
                contextWalkFilter,
                progressTracker
            );

            progressTracker.ClearProgressBar();
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

            InjectionProgressTracker progressTracker = new(totalSegments: (sceneBatchItems.Count + prefabBatchItems.Count) * 10);
            Stopwatch sceneStopwatch = Stopwatch.StartNew();

            InjectionResults sceneResults = RunSceneBatch
            (
                sceneBatchItems,
                progressTracker
            );

            sceneStopwatch.Stop();
            Stopwatch prefabStopwatch = Stopwatch.StartNew();

            InjectionResults prefabResults = RunPrefabBatch
            (
                prefabBatchItems,
                progressTracker
            );

            prefabStopwatch.Stop();
            progressTracker.ClearProgressBar();

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

        private static InjectionResults RunSceneBatch(
            IEnumerable<SceneBatchItem> batchItems,
            InjectionProgressTracker progressTracker)
        {
            batchItems = batchItems.ToArray();
            InjectionResults combinedResults = new();
            string initialScenePath = SceneManager.GetActiveScene().path;

            if (batchItems.Any())
                Logger.LogBatchInjectHeader(batchItems);

            foreach (SceneBatchItem item in batchItems)
            {
                Scene scene = EditorSceneManager.OpenScene
                (
                    item.Path,
                    OpenSceneMode.Single
                );

                progressTracker.SetTitle($"Batch injecting scene: {scene.name}");

                IEnumerable<Transform> startObjects = scene
                    .GetRootGameObjects()
                    .Select(x => x.transform);

                using (new Logger.BatchInjectionScope(item))
                {
                    InjectionResults results = RunContext
                    (
                        startObjects,
                        item.ContextWalkFilter,
                        progressTracker
                    );

                    combinedResults.AddToResults(results);
                }

                EditorSceneManager.SaveScene(scene);
            }

            if (!string.IsNullOrEmpty(initialScenePath))
                EditorSceneManager.OpenScene
                (
                    initialScenePath,
                    OpenSceneMode.Single
                );

            return combinedResults;
        }

        private static InjectionResults RunPrefabBatch(
            IEnumerable<PrefabBatchItem> batchItems,
            InjectionProgressTracker progressTracker)
        {
            batchItems = batchItems.ToArray();
            InjectionResults combinedResults = new();

            if (batchItems.Any())
                Logger.LogBatchInjectHeader(batchItems);

            foreach (PrefabBatchItem item in batchItems)
            {
                Transform prefabRoot = AssetDatabase
                    .LoadAssetAtPath<GameObject>(item.Path)
                    .transform
                    .root;

                progressTracker.SetTitle($"Batch injecting prefab: {prefabRoot.name}");

                Transform[] startObjects =
                {
                    prefabRoot
                };

                using (new Logger.BatchInjectionScope(item))
                {
                    InjectionResults results = RunContext
                    (
                        startObjects,
                        ContextWalkFilter.PrefabAssets,
                        progressTracker
                    );

                    combinedResults.AddToResults(results);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return combinedResults;
        }

        private static InjectionResults RunContext(
            IEnumerable<Object> startObjects,
            ContextWalkFilter contextWalkFilter,
            InjectionProgressTracker progressTracker)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Transform[] startTransforms = startObjects switch
            {
                Transform[] t => t,
                IEnumerable<Transform> t => t.ToArray(),
                IEnumerable<GameObject> g => g.Select(x => x.transform).ToArray(),
                IEnumerable<Component> c => c.Select(x => x.transform).ToArray(),
                _ => throw new Exception("Unsupported start object type.")
            };

            progressTracker.BeginSegment(stepCount: 1);
            progressTracker.UpdateInfoText("Building injection graph");
            InjectionGraph injectionGraph = new(startTransforms);
            progressTracker.NextStep();

            IReadOnlyCollection<TransformNode> activeTransformNodes = GraphFilter.ApplyWalkFilter
            (
                injectionGraph,
                startTransforms,
                contextWalkFilter,
                progressTracker
            );

            InjectionContext context = new
            (
                activeTransformNodes,
                progressTracker
            );

            BindingValidator.ValidateBindings
            (
                context,
                progressTracker
            );

            Resolver.Resolve
            (
                context,
                progressTracker
            );

            Injector.InjectDependencies
            (
                context,
                progressTracker
            );

            ProxySwapTargetCollector.CollectSwapTargets
            (
                context,
                progressTracker
            );

            InjectionResults results = context.GetResults();
            stopwatch.Stop();
            Logger.LogResults(results);

            Logger.LogSummary
            (
                prefix: "Injection complete",
                results,
                stopwatch.ElapsedMilliseconds
            );

            /*InjectionContextJsonProjector.SaveToDisk(context, injectionGraph);*/ // TODO: Make batch friendly
            return results;
        }
    }
}