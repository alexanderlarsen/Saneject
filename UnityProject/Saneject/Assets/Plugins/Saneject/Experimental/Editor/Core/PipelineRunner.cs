using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Json;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class PipelineRunner
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            InjectionSession session = InjectionSession.StartSession(startGameObject);

            BindingConfigValidator.ValidateBindings(session);

            if (ProxyCreator.ShouldCreateProxies(session))
            {
                ProxyCreator.CreateAllProxies(session, out int proxiesCreated);
                Logger.LogProxyCreationCompleted(proxiesCreated);
                return;
            }

            DependencyResolver.Resolve(session);
            DependencyInjector.InjectDependencies(session);

            session.StopSession();

            Logger.TryClearLog();
            Logger.LogErrors(session);
            Logger.LogUnusedBindings(session);
            Logger.LogStats(session);

            InjectionGraphJsonExporter.SaveGraphToJson(session.Graph);
        }
    }
}