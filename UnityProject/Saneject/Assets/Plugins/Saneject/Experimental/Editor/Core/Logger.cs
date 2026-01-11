using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using Plugins.Saneject.Experimental.Editor.Utility;
using Plugins.Saneject.Experimental.Runtime.Settings;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            IEnumerable<Error> errors = context
                .Errors
                .OrderBy(e => ErrorPriority(e.ErrorType))
                .Where(error => !error.SuppressError);

            foreach (Error error in errors)
                if (error.Exception != null)
                {
                    Debug.LogError($"Saneject: {ErrorTypeToDisplayString(error.ErrorType)} (exception details in next log) {error.ErrorMessage}", error.LogContext);
                    Debug.LogException(error.Exception, error.LogContext);
                }
                else
                {
                    Debug.LogError($"Saneject: {ErrorTypeToDisplayString(error.ErrorType)} {error.ErrorMessage}", error.LogContext);
                }

            return;

            static int ErrorPriority(ErrorType errorType)
            {
                return errorType switch
                {
                    ErrorType.InvalidBinding => 0,
                    ErrorType.MissingBinding => 1,
                    ErrorType.MissingGlobalObject => 2,
                    ErrorType.MissingDependency => 3,
                    ErrorType.MissingDependencies => 4,
                    ErrorType.BindingFilterException => 5,
                    ErrorType.MethodInvocationException => 6,
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
                    ErrorType.BindingFilterException => "Binding filter exception",
                    ErrorType.MethodInvocationException => "Method invocation exception",
                    _ => "Unknown error"
                };
            }
        }

        public static void LogUnusedBindings(InjectionContext context)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            foreach (BindingNode binding in context.UnusedBindings)
                Debug.LogWarning($"Saneject: Unused binding {SignatureBuilder.GetBindingSignature(binding)}. If you don't plan to use this binding, you can safely remove it.", binding.ScopeNode.TransformNode.Transform);
        }

        public static void LogCreatedProxyAssets(InjectionContext context)
        {
            foreach ((string path, Object instance) asset in context.CreatedProxyAssets)
                Debug.Log($"Saneject: Created proxy asset at '{asset.path}'.", asset.instance);
        }

        public static void LogSummary(InjectionContext context)
        {
            if (!UserSettings.LogInjectionSummary)
                return;

            InjectionSummary summary = new(context);
            StringBuilder sb = new();

            sb.Append("Saneject: Injection complete | ")
                .AppendQuantity(summary.ScopesProcessedCount, "scope processed", "scopes processed", " | ")
                .AppendQuantity(summary.GlobalRegistrationCount, "global registered", "globals registered", " | ")
                .AppendQuantity(summary.InjectedFieldCount, "field injected", "fields injected", " | ")
                .AppendQuantity(summary.InjectedPropertyCount, "property injected", "properties injected", " | ")
                .AppendQuantity(summary.InjectedMethodCount, "method injected", "methods injected", " | ")
                .AppendQuantity(summary.MissingDependencyCount, "missing dependency", "missing dependencies", " | ")
                .AppendQuantity(summary.MissingBindingCount, "missing binding", "missing bindings", " | ")
                .AppendQuantity(summary.InvalidBindingCount, "invalid binding", "invalid bindings", " | ")
                .AppendQuantity(summary.UnusedBindingCount, "unused binding", "unused bindings", " | ")
                .Append("Completed in ")
                .Append(summary.ElapsedMilliseconds)
                .Append(" ms");

            if (summary.SuppressedErrorCount > 0)
                sb.Append("\n⚠️ ")
                    .Append(summary.SuppressedErrorCount)
                    .Append(" missing binding/dependency ")
                    .AppendQuantity(summary.SuppressedErrorCount, "error was", "errors were", addCount: false)
                    .Append(" suppressed due to [Inject(suppressMissingErrors: true)]. Remove the flag to view detailed logs.");

            switch (summary.GetLogSeverity())
            {
                case LogSeverity.Info:
                    Debug.Log(sb.ToString());
                    break;

                case LogSeverity.Warning:
                    Debug.LogWarning(sb.ToString());
                    break;

                case LogSeverity.Error:
                    Debug.LogError(sb.ToString());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            StringBuilder sb = new();

            sb.Append("Saneject: ")
                .AppendQuantity(count, "proxy script", "proxy scripts")
                .Append(" generated. Unity has recompiled and stopped the injection pass. Inject again to complete.");

            Debug.LogWarning(sb.ToString());
        }
    }
}