using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class Logger
    {
        public static void LogErrors(IReadOnlyList<Error> errors)
        {
            foreach (Error error in errors.OrderBy(e => ErrorPriority(e.ErrorType)))
                Debug.LogError($"Saneject: {error.ErrorMessage}", error.LogContext);

            return;

            static int ErrorPriority(ErrorType type)
            {
                return type switch
                {
                    ErrorType.InvalidBinding => 0,
                    ErrorType.MissingBinding => 1,
                    ErrorType.MissingDependency => 2,
                    _ => int.MaxValue
                };
            }
        }

        public static void LogUnusedBindings(InjectionGraph graph)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            foreach (BindingNode binding in graph.EnumerateAllBindingNodes().Where(binding => !binding.IsUsed))
                Debug.LogWarning($"Saneject: Unused binding {SignatureBuilder.GetBindingSignature(binding)}. If you don't plan to use this binding, you can safely remove it.", binding.ScopeNode.TransformNode.Transform);
        }

        public static void LogStats(int elapsedMilliseconds)
        {
            Debug.Log($"Saneject: Injection took {elapsedMilliseconds}ms.");
        }

        public static void LogProxyCreationCompleted(int numProxiesCreated)
        {
            Debug.Log($"Saneject: Created {numProxiesCreated} proxies. Inject again to finish injection.");
        }
    }
}