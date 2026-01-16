using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Json;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionRunner
    {
        public static void Run(
            IEnumerable<GameObject> startGameObjects,
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();
            InjectionContext context = new(startGameObjects, walkFilter);
            BindingValidator.ValidateBindings(context);

            if (ProxyProcessor.CreateProxies(context) != ProxyCreationResult.Ready)
                return;

            Resolver.Resolve(context);
            Injector.InjectDependencies(context);
            context.StopTimer();
            context.CacheUnusedBindings();
            Logger.LogErrors(context);
            Logger.LogUnusedBindings(context);
            Logger.LogCreatedProxyAssets(context);
            Logger.LogSummary(context);
            InjectionContextJsonProjector.SaveToDisk(context);
        }
    }
}