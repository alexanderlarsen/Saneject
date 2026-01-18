using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
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
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();
            InjectionResults results = RunContext(startObjects, walkFilter);
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

                IEnumerable<Transform> startObjects = EditorSceneManager
                    .OpenScene(path, OpenSceneMode.Single)
                    .GetRootGameObjects()
                    .Select(x => x.transform);

                InjectionResults results = RunContext(startObjects, walkFilter);
                sceneResults.AddToResults(results);
            }

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

                InjectionResults results = RunContext(startObjects, walkFilter);
                prefabResults.AddToResults(results);
            }

            Logger.LogSummary(sceneResults);
            Logger.LogSummary(prefabResults);
        }

        private static InjectionResults RunContext(
            IEnumerable<Object> startObjects,
            WalkFilter walkFilter)
        {
            Transform[] startTransforms = startObjects switch
            {
                Transform[] t => t,
                IEnumerable<Transform> t => t.ToArray(),
                IEnumerable<GameObject> g => g.Select(x => x.transform).ToArray(),
                IEnumerable<Component> c => c.Select(x => x.transform).ToArray(),
                _ => throw new Exception("Unsupported start object type.")
            };

            InjectionContext context = new(startTransforms, walkFilter);
            BindingValidator.ValidateBindings(context);
            Resolver.Resolve(context);
            Injector.InjectDependencies(context);
            InjectionResults results = context.GetResults();
            Logger.LogResults(results);
            InjectionContextJsonProjector.SaveToDisk(context); // TODO: Make batch friendly
            return results;
        }
    }
}