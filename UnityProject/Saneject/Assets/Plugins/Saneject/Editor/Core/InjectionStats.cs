using System.Text;
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
            StringBuilder sb = new();

            sb.Append("Saneject: ");
            sb.Append(injectionType);
            sb.Append(" injection complete | ");
            sb.Append(numScopesProcessed);
            sb.Append(numScopesProcessed == 1 ? " scope processed | " : " scopes processed | ");
            sb.Append(numInjectedGlobal);
            sb.Append(numInjectedGlobal == 1 ? " global dependency | " : " global dependencies | ");
            sb.Append(numInjectedFields);
            sb.Append(numInjectedFields == 1 ? " injected field | " : " injected fields | ");
            sb.Append(numMissingBindings);
            sb.Append(numMissingBindings == 1 ? " missing binding | " : " missing bindings | ");
            sb.Append(numInvalidBindings);
            sb.Append(numInvalidBindings == 1 ? " invalid binding | " : " invalid bindings | ");
            sb.Append(numUnusedBindings);
            sb.Append(numUnusedBindings == 1 ? " unused binding | " : " unused bindings | ");
            sb.Append(numMissingDependencies);
            sb.Append(numMissingDependencies == 1 ? " missing dependency | " : " missing dependencies | ");
            sb.Append("Completed in ");
            sb.Append(elapsedMilliseconds);
            sb.Append(" ms");

            string log = sb.ToString();

            switch (GetLogSeverity())
            {
                case LogSeverity.Info:
                    Debug.Log(log);
                    break;

                case LogSeverity.Warning:
                    Debug.LogWarning(log);
                    break;

                case LogSeverity.Error:
                    Debug.LogError(log);
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