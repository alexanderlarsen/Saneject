using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionSummary
    {
        public InjectionSummary(InjectionResults results)
        {
            GlobalRegistrationCount = results.GlobalRegistrationCount;
            InjectedFieldCount = results.InjectedFieldCount;
            InjectedPropertyCount = results.InjectedPropertyCount;
            InjectedMethodCount = results.InjectedMethodCount;

            MissingBindingCount = results
                .Errors
                .Count(error => error.ErrorType == ErrorType.MissingBinding && !error.SuppressError);

            UnusedBindingCount = results
                .UnusedBindingNodes
                .Count;

            InvalidBindingCount = results
                .Errors
                .Count(error => error.ErrorType == ErrorType.InvalidBinding && !error.SuppressError);

            MissingDependencyCount = results
                .Errors
                .Count(error => error.ErrorType is ErrorType.MissingDependency or ErrorType.MissingDependencies && !error.SuppressError);

            SuppressedErrorCount = results
                .Errors
                .Count(error => error.SuppressError);

            ScopesProcessedCount = results.ScopesProcessedCount;
            ElapsedMilliseconds = results.ElapsedMilliseconds;
        }

        public int ScopesProcessedCount { get; }
        public int GlobalRegistrationCount { get; }
        public int InjectedFieldCount { get; }
        public int InjectedPropertyCount { get; }
        public int InjectedMethodCount { get; }
        public int MissingDependencyCount { get; }
        public int MissingBindingCount { get; }
        public int InvalidBindingCount { get; }
        public int UnusedBindingCount { get; }
        public int SuppressedErrorCount { get; }
        public long ElapsedMilliseconds { get; }

        public LogSeverity GetLogSeverity()
        {
            int totalErrors = MissingBindingCount + InvalidBindingCount + MissingDependencyCount;

            if (totalErrors > 0)
                return LogSeverity.Error;

            return UnusedBindingCount > 0 ? LogSeverity.Warning : LogSeverity.Info;
        }
    }
}