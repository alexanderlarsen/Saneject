using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Editor.Utility;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utils;
using Plugins.Saneject.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    [InitializeOnLoad]
    public static class Logger
    {
        private const string ProxyStubCreationCountKey = "Saneject.ProxyStubCreationCount";

        static Logger()
        {
            AssemblyReloadEvents.afterAssemblyReload += TryLogProxyStubsCreated;
        }

        public static void TryClearLog()
        {
            if (UserSettings.ClearLogsOnInjection)
                ConsoleUtils.ClearLog();
        }

        public static void LogErrors(InjectionSession session)
        {
            foreach (Error error in session.Errors.OrderBy(e => ErrorPriority(e.ErrorType)))
                Debug.LogError($"Saneject: {error.ErrorMessage}", error.LogContext);

            return;

            static int ErrorPriority(ErrorType type)
            {
                return type switch
                {
                    ErrorType.InvalidBinding => 0,
                    ErrorType.MissingBinding => 1,
                    ErrorType.MissingGlobalDependency => 2,
                    ErrorType.MissingDependency => 3,
                    _ => int.MaxValue
                };
            }
        }

        public static void LogUnusedBindings(InjectionSession session)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            IEnumerable<BindingNode> unusedBindings = session
                .Graph
                .EnumerateAllBindingNodes()
                .Where(binding => !session.UsedBindings.Contains(binding));

            foreach (BindingNode binding in unusedBindings)
                Debug.LogWarning($"Saneject: Unused binding {SignatureBuilder.GetBindingSignature(binding)}. If you don't plan to use this binding, you can safely remove it.", binding.ScopeNode.TransformNode.Transform);
        }

        public static void LogCreatedProxyAssets(InjectionSession session)
        {
            foreach ((string path, Object instance) asset in session.CreatedProxyAssets)
                Debug.Log($"Saneject: Created proxy asset at '{asset.path}'.", asset.instance);
        }

        public static void LogStats(InjectionSession session)
        {
            Debug.Log($"Saneject: Injection took {session.DurationMilliseconds}ms | ID: {session.Id}.");
        }

        public static void SaveProxyStubCreationCount(int count)
        {
            SessionState.SetInt(ProxyStubCreationCountKey, count);
        }

        private static void TryLogProxyStubsCreated()
        {
            int count = SessionState.GetInt(ProxyStubCreationCountKey, 0);
            SessionState.EraseInt(ProxyStubCreationCountKey);

            if (count <= 0)
                return;

            string scriptsWord = count == 1 ? "script" : "scripts";

            Debug.LogWarning($"Saneject: {count} proxy {scriptsWord} generated. Unity has recompiled and stopped the injection pass. Inject again to complete.");
        }
    }
}