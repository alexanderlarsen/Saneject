using System.Collections.Generic;
using System.Diagnostics;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Json;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class PipelineRunner
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            InjectionGraph graph = InjectionGraphFactory.CreateGraph(startGameObject);
            List<Error> errors = new();

            BindingConfigValidator.ValidateBindings
            (
                graph,
                errors
            );

            if (ProxyCreator.ShouldCreateProxies(graph))
            {
                ProxyCreator.CreateAllProxies(graph, out int proxiesCreated);
                Logger.LogProxyCreationCompleted(proxiesCreated);
                return;
            }

            DependencyResolver.Resolve
            (
                graph,
                errors,
                out InjectionPlan injectionPlan
            );

            DependencyInjector.InjectDependencies(injectionPlan);

            stopwatch.Stop();

            Logger.LogErrors(errors);
            Logger.LogUnusedBindings(graph);
            Logger.LogStats(stopwatch.Elapsed.Milliseconds);

            InjectionGraphJsonExporter.SaveGraphToJson(graph);
        }
    }
}