using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Json;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class PipelineRunner
    {
        public static void InjectSingleHierarchy(GameObject startGameObject)
        {
            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();

            InjectionSession session = InjectionSession.Create(startGameObject);
            session.StartTimer();

            BindingConfigValidator.ValidateBindings(session);

            if (ProxyCreator.ShouldCreateProxies(session))
            {
                ProxyCreator.CreateAllProxies(session, out int proxiesCreated);
                Logger.LogProxyCreationCompleted(proxiesCreated);
                return;
            }

            DependencyResolver.Resolve(session);
            DependencyInjector.InjectDependencies(session);

            session.StopTimer();

            Logger.LogErrors(session);
            Logger.LogUnusedBindings(session);
            Logger.LogStats(session);

            InjectionGraphJsonExporter.SaveGraphToJson(session.Graph);
        }
    }
}