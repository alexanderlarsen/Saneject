using System.Collections.Generic;
using System.Diagnostics;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Json;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class PipelineRunner
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            InjectionGraph graph = InjectionGraphFactory.CreateGraph(startGameObject);

            BindingValidator.ValidateBindings
            (
                graph,
                out IReadOnlyList<BindingError> bindingsErrors
            );

            DependencyResolver.Resolve(
                graph,
                out InjectionPlan injectionPlan,
                out IReadOnlyList<DependencyError> dependencyErrors
            );

            DependencyInjector.InjectDependencies(injectionPlan);

            stopwatch.Stop();
            
            Logger.LogBindingErrors(bindingsErrors);
            Logger.LogDependencyErrors(dependencyErrors);
            Logger.LogUnusedBindings(graph);
            Logger.LogStats(stopwatch.Elapsed.Milliseconds);

            InjectionGraphJsonExporter.SaveGraphToJson(graph);
        }
    }
}