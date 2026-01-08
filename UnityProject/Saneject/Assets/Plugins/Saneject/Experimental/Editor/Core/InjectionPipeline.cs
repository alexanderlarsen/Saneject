using System.Diagnostics;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Json;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionPipeline
    {
        public static void Inject(params GameObject[] startGameObjects)
        {
            Logger.TryClearLog();
            InjectionContext context = InjectionContext.Create(startGameObjects);
            BindingValidator.ValidateBindings(context);

            if (ProxyProcessor.CreateProxies(context) != ProxyCreationResult.Ready)
                return;

            Resolver.Resolve(context);
            Injector.InjectDependencies(context);
            context.StopTimer();
            Logger.LogErrors(context);
            Logger.LogUnusedBindings(context);
            Logger.LogCreatedProxyAssets(context);
            Logger.LogStats(context);
            InjectionGraphJsonExporter.SaveGraphToJson(context.Graph);
        }
    }
}