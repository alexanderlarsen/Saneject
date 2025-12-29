using System.Collections.Generic;
using Plugins.Saneject.Experimental.GraphSystem.Data;
using Plugins.Saneject.Experimental.GraphSystem.Debugging;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem
{
    public static class NewDependencyInjector
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            InjectionGraph graph = new(startGameObject);
            NewBindingValidator.ValidateBindings(graph, out IReadOnlyList<BindingError> bindingsErrors);

            // ... other stuff

            NewLogger.LogBindingErrors(bindingsErrors);

            const string path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\GraphSystem\graph.json";

            InjectionGraphJsonExporter.SaveGraphToJson(graph, path);
        }
    }
}