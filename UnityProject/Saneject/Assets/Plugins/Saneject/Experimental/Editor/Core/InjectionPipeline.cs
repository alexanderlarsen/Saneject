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
            InjectionSession session = InjectionSession.StartSession(startGameObjects);
            BindingValidator.ValidateBindings(session);

            if (ProxyProcessor.CreateProxies(session) != ProxyCreationResult.Ready)
                return;

            Resolver.Resolve(session);
            Injector.InjectDependencies(session);

            session.EndSession();

            Logger.LogErrors(session);
            Logger.LogUnusedBindings(session);
            Logger.LogCreatedProxyAssets(session);
            Logger.LogStats(session);

            InjectionGraphJsonExporter.SaveGraphToJson(session.Graph);
        }
    }
}