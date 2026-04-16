using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Plugins.Saneject.Editor.Data.BatchInjection;
using Plugins.Saneject.Editor.Data.Errors;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using Plugins.Saneject.Editor.Data.Injection;
using Plugins.Saneject.Editor.Data.Logging;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Editor.Utilities;
using Plugins.Saneject.Runtime.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Saneject.Editor.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Logger
    {
        private const string BatchInjectHeaderLine = "────────────";

        public static void TryClearLog()
        {
            if (!UserSettings.ClearLogsOnInjection)
                return;

            ConsoleUtility.ClearLog();
        }

        public static void LogResults(InjectionResults results)
        {
            LogErrors(results);
            LogUnusedBindings(results);
            LogCreatedProxyAssets(results);
        }

        public static void LogSummary(
            string prefix,
            InjectionResults results,
            long elapsedMilliseconds)
        {
            if (results.ScopesProcessedCount == 0)
            {
                LogNoScopesFound(prefix);
                return;
            }

            if (!UserSettings.LogInjectionSummary)
                return;

            InjectionSummary summary = new(results);
            StringBuilder sb = new();

            sb.Append($"Saneject: {prefix} | ")
                .AppendQuantity(summary.ScopesProcessedCount, "scope processed", "scopes processed", " | ")
                .AppendQuantity(summary.GlobalRegistrationCount, "global registered", "globals registered", " | ")
                .AppendQuantity(summary.ProxySwapTargetsCount, "proxy swap target registered", "proxy swap targets registered", " | ")
                .AppendQuantity(summary.InjectedFieldCount, "field injected", "fields injected", " | ")
                .AppendQuantity(summary.InjectedPropertyCount, "property injected", "properties injected", " | ")
                .AppendQuantity(summary.InjectedMethodCount, "method injected", "methods injected", " | ")
                .AppendQuantity(summary.MissingDependencyCount, "missing dependency", "missing dependencies", " | ")
                .AppendQuantity(summary.MissingBindingCount, "missing binding", "missing bindings", " | ")
                .AppendQuantity(summary.InvalidBindingCount, "invalid binding", "invalid bindings", " | ")
                .AppendQuantity(summary.UnusedBindingCount, "unused binding", "unused bindings", " | ")
                .Append("Completed in ")
                .Append(elapsedMilliseconds)
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

        public static void LogNoScopesFound(string prefix)
        {
            Debug.Log($"Saneject: {prefix}. No scopes were found. Nothing was injected.");
        }

        public static void LogInjectionCancelledByUser()
        {
            Debug.Log("Saneject: Injection cancelled by user.");
        }

        public static void LogBatchInjectHeader(IEnumerable<SceneBatchItem> batchItems)
        {
            int count = batchItems.Count();
            Debug.Log($"<b>{BatchInjectHeaderLine}  Saneject: Scene batch injection ({count} {(count == 1 ? "scene" : "scenes")})  {BatchInjectHeaderLine}</b>");
        }

        public static void LogBatchInjectHeader(IEnumerable<PrefabBatchItem> batchItems)
        {
            int count = batchItems.Count();
            Debug.Log($"<b>{BatchInjectHeaderLine}  Saneject: Prefab batch injection ({count} {(count == 1 ? "prefab" : "prefabs")})  {BatchInjectHeaderLine}</b>");
        }

        public static void LogBatchSummary(
            int sceneBatchItemsCount,
            int prefabBatchItemsCount,
            InjectionResults sceneResults,
            InjectionResults prefabResults,
            long sceneElapsedMilliseconds,
            long prefabElapsedMilliseconds)
        {
            if (sceneBatchItemsCount + prefabBatchItemsCount == 0)
            {
                LogNoScopesFound("Batch injection complete");
                return;
            }

            Debug.Log($"<b>{BatchInjectHeaderLine}  Saneject: Batch injection summary  {BatchInjectHeaderLine}</b>");

            if (sceneBatchItemsCount > 0)
                LogSummary
                (
                    "Scene batch injection complete",
                    sceneResults,
                    sceneElapsedMilliseconds
                );

            if (prefabBatchItemsCount > 0)
                LogSummary
                (
                    "Prefab batch injection complete",
                    prefabResults,
                    prefabElapsedMilliseconds
                );
        }

        private static void LogErrors(InjectionResults results)
        {
            IEnumerable<InjectionError> errors = results
                .Errors
                .OrderBy(ErrorPriority)
                .Where(error => !error.SuppressError)
                .ToArray();

            foreach (InjectionError error in errors)
            {
                StringBuilder errorStringBuilder = new();

                switch (error)
                {
                    case InvalidBindingError invalidBindingError:
                    {
                        errorStringBuilder.Append("Invalid binding. ");
                        errorStringBuilder.Append(invalidBindingError.Reason);
                        errorStringBuilder.Append(" ");
                        errorStringBuilder.Append(invalidBindingError.BindingSignature);
                        break;
                    }
                    case MissingBindingError missingBindingError:
                    {
                        errorStringBuilder.Append("Missing binding. Expected something like ");
                        errorStringBuilder.Append(missingBindingError.ExpectedBindingSignature);
                        errorStringBuilder.Append(" ");
                        errorStringBuilder.Append(missingBindingError.SiteSignature);
                        break;
                    }

                    case MissingGlobalDependencyError missingGlobalDependencyError:
                    {
                        errorStringBuilder.Append("Could not locate global object. ");
                        errorStringBuilder.Append(missingGlobalDependencyError.BindingSignature);

                        if (missingGlobalDependencyError.RejectedTypes is { Count: > 0 })
                        {
                            string typeList = string.Join(", ", missingGlobalDependencyError.RejectedTypes.Select(t => t.Name));
                            errorStringBuilder.AppendLine();
                            errorStringBuilder.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                            errorStringBuilder.AppendLine("Use RuntimeProxy for cross-context references or disable Context Isolation in Settings.");
                        }

                        break;
                    }

                    case MissingDependencyError missingDependencyError:
                    {
                        errorStringBuilder.Append($"Could not locate {(missingDependencyError.IsCollection ? "dependencies" : "dependency")}. ");
                        errorStringBuilder.Append(missingDependencyError.BindingSignature);
                        errorStringBuilder.Append(" ");
                        errorStringBuilder.Append(missingDependencyError.SiteSignature);

                        if (missingDependencyError.RejectedTypes is { Count: > 0 })
                        {
                            string typeList = string.Join(", ", missingDependencyError.RejectedTypes.Select(t => t.Name));
                            errorStringBuilder.AppendLine();
                            errorStringBuilder.AppendLine($"Candidates rejected due to scene/prefab or prefab/prefab context mismatch: {typeList}.");
                            errorStringBuilder.AppendLine("Use RuntimeProxy for cross-context references or disable Context Isolation in Settings.");
                        }

                        break;
                    }

                    case FilterCandidatesError filterCandidatesError:
                    {
                        errorStringBuilder.Append("Filter candidates error ");
                        errorStringBuilder.Append(filterCandidatesError.BindingSignature);
                        break;
                    }

                    case InjectMethodInvocationError injectMethodInvocationError:
                    {
                        errorStringBuilder.Append("Inject method invocation error ");
                        errorStringBuilder.Append(injectMethodInvocationError.SiteSignature);
                        break;
                    }
                }

                if (error.Exception != null)
                    errorStringBuilder.Append(" | Exception details in next log:");

                Debug.LogError($"Saneject: {errorStringBuilder}", error.LogContext);

                if (error.Exception != null)
                    Debug.LogException(error.Exception, error.LogContext);
            }

            return;

            static int ErrorPriority(InjectionError error)
            {
                return error switch
                {
                    InvalidBindingError => 0,
                    MissingBindingError => 1,
                    MissingGlobalDependencyError => 2,
                    MissingDependencyError => 3,
                    FilterCandidatesError => 4,
                    InjectMethodInvocationError => 5,
                    _ => int.MaxValue
                };
            }
        }

        private static void LogUnusedBindings(InjectionResults results)
        {
            if (!UserSettings.LogUnusedBindings)
                return;

            foreach (BindingNode binding in results.UnusedBindingNodes)
                Debug.LogWarning($"Saneject: Unused binding {SignatureUtility.GetBindingSignature(binding)}. If you don't plan to use this binding, you can safely remove it.", binding.ScopeNode.TransformNode.Transform);
        }

        private static void LogCreatedProxyAssets(InjectionResults results)
        {
            foreach ((string path, Object instance) asset in results.CreatedProxyAssets)
                Debug.Log($"Saneject: Created proxy asset at '{asset.path}'.", asset.instance);
        }

        public readonly struct BatchInjectionScope : IDisposable
        {
            private static readonly string[] LogColors =
            {
                "#E57373", // soft red
                "#81C784", // soft green
                "#64B5F6", // soft blue
                "#FFD54F", // soft amber/yellow
                "#BA68C8", // soft magenta
                "#4DD0E1" // soft cyan
            };

            private static int logColorIndex;

            private readonly string contents;
            private readonly string color;

            public BatchInjectionScope(SceneBatchItem batchItem)
            {
                color = PickColor();
                contents = $"scene injection: {batchItem.Path}";
                Debug.Log($"<color={color}>↓↓↓ Start {contents} ↓↓↓</color>");
            }

            public BatchInjectionScope(PrefabBatchItem batchItem)
            {
                color = PickColor();
                contents = $"prefab injection: {batchItem.Path}";
                Debug.Log($"<color={color}>↓↓↓ Start {contents} ↓↓↓</color>");
            }

            public void Dispose()
            {
                Debug.Log($"<color={color}>↑↑↑ End {contents} ↑↑↑</color>");
            }

            private static string PickColor()
            {
                int index = logColorIndex;
                logColorIndex = (logColorIndex + 1) % LogColors.Length;
                return LogColors[index];
            }
        }
    }
}