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

        public static void LogErrors(InjectionContext context)
        {
            foreach (Error error in context.Errors.OrderBy(e => ErrorPriority(e.ErrorType)))
                Debug.LogError($"Saneject: {ErrorTypeToDisplayString(error.ErrorType)} {error.ErrorMessage}", error.LogContext);

            return;

            static int ErrorPriority(ErrorType errorType)
            {
                return errorType switch
                {
                    ErrorType.InvalidBinding => 0,
                    ErrorType.MissingBinding => 1,
                    ErrorType.MissingGlobalObject => 2,
                    ErrorType.MissingDependency => 3,
                    ErrorType.MissingDependencies => 3,
                    ErrorType.MethodInvocationException => 4,
                    _ => int.MaxValue
                };
            }

            static string ErrorTypeToDisplayString(ErrorType type)
            {
                return type switch
                {
                    ErrorType.InvalidBinding => "Invalid binding",
                    ErrorType.MissingBinding => "Missing binding. Expected something like",
                    ErrorType.MissingGlobalObject => "Could not locate global object",
                    ErrorType.MissingDependency => "Could not locate dependency",
                    ErrorType.MissingDependencies => "Could not locate dependencies",
                    ErrorType.MethodInvocationException => "Method invocation exception",
                    _ => "Unknown error"
                };
            }
        }

        public static void LogUnusedBindings(InjectionContext context)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            IEnumerable<BindingNode> unusedBindings = context
                .Graph
                .EnumerateAllBindingNodes()
                .Where(binding => !context.UsedBindings.Contains(binding));

            foreach (BindingNode binding in unusedBindings)
                Debug.LogWarning($"Saneject: Unused binding {SignatureBuilder.GetBindingSignature(binding)}. If you don't plan to use this binding, you can safely remove it.", binding.ScopeNode.TransformNode.Transform);
        }

        public static void LogCreatedProxyAssets(InjectionContext context)
        {
            foreach ((string path, Object instance) asset in context.CreatedProxyAssets)
                Debug.Log($"Saneject: Created proxy asset at '{asset.path}'.", asset.instance);
        }

        public static void LogStats(InjectionContext context)
        {
            Debug.Log($"Saneject: Injection took {context.ElapsedMilliseconds}ms | ID: {context.Id}.");
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