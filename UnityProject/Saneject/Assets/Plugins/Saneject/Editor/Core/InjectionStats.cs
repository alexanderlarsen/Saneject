using UnityEngine;

namespace Plugins.Saneject.Editor.Core
{
    /// <summary>
    /// Class for collecting injection stats for logging purposes.
    /// </summary>
    public class InjectionStats
    {
        public int numInjectedGlobal;
        public int numInjectedFields;
        public int numMissingBindings;
        public int numUnusedBindings;
        public int numInvalidBindings;
        public int numMissingDependencies;

        private enum LogSeverity
        {
            Info,
            Warning,
            Error
        }

        public void LogStats(
            string injectionType,
            int numScopesProcessed,
            long elapsedMilliseconds)
        {
            string message = $"Saneject: {injectionType} injection complete | {numScopesProcessed} scopes processed | {numInjectedGlobal} global dependencies | {numInjectedFields} injected fields | {numMissingBindings} missing bindings | {numInvalidBindings} invalid bindings | {numUnusedBindings} unused bindings | {numMissingDependencies} missing dependencies | Completed in {elapsedMilliseconds} ms";

            switch (GetLogSeverity())
            {
                case LogSeverity.Info:
                    Debug.Log(message);
                    break;

                case LogSeverity.Warning:
                    Debug.LogWarning(message);
                    break;

                case LogSeverity.Error:
                    Debug.LogError(message);
                    break;
            }
        }

        private LogSeverity GetLogSeverity()
        {
            int totalErrors = numMissingBindings + numInvalidBindings + numMissingDependencies;

            if (totalErrors > 0)
                return LogSeverity.Error;

            return numUnusedBindings > 0 ? LogSeverity.Warning : LogSeverity.Info;
        }
    }
}