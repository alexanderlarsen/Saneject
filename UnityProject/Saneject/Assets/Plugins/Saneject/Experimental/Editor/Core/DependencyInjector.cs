using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyInjector
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            InjectionGraph graph = new(startGameObject);
            BindingValidator.ValidateBindings(graph, out IReadOnlyList<BindingError> bindingsErrors);

            // ... other stuff

            Logger.LogBindingErrors(bindingsErrors);

            const string path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\graph.json";

            InjectionGraphJsonExporter.SaveGraphToJson(graph, path);
        }
    }
}