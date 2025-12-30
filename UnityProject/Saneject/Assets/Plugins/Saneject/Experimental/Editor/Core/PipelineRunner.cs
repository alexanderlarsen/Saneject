using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class PipelineRunner
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
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

            Logger.LogBindingErrors(bindingsErrors);
            Logger.LogDependencyErrors(dependencyErrors);

            InjectionGraphJsonExporter.SaveGraphToJson(graph);
        }
    }
}