using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Json;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class InjectionPipeline
    {
        public static void Inject(
            GameObject[] startGameObjects,
            WalkFilter walkFilter)
        {
            Inject
            (
                startGameObjects.Select(gameObject => gameObject.transform).ToArray(),
                walkFilter
            );
        }

        public static void Inject(
            Transform[] startTransforms,
            WalkFilter walkFilter)
        {
            Logger.TryClearLog();
            InjectionContext context = new(startTransforms, walkFilter);
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