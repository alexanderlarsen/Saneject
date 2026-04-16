using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Errors;
using Plugins.Saneject.Editor.Data.Logging;

namespace Plugins.Saneject.Editor.Data.Injection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class InjectionSummary
    {
        public InjectionSummary(InjectionResults results)
        {
            GlobalRegistrationCount = results.GlobalRegistrationCount;
            ProxySwapTargetsCount = results.ProxySwapTargetsCount;
            InjectedFieldCount = results.InjectedFieldCount;
            InjectedPropertyCount = results.InjectedPropertyCount;
            InjectedMethodCount = results.InjectedMethodCount;
            Errors = results.Errors;

            MissingBindingCount = Errors
                .OfType<MissingBindingError>()
                .Count(error => !error.SuppressError);

            InvalidBindingCount = Errors
                .OfType<InvalidBindingError>()
                .Count(error => !error.SuppressError);

            MissingDependencyCount = Errors
                .OfType<MissingDependencyError>()
                .Count(error => !error.SuppressError);

            SuppressedErrorCount = Errors
                .Count(error => error.SuppressError);

            UnusedBindingCount = results
                .UnusedBindingNodes
                .Count;

            ScopesProcessedCount = results.ScopesProcessedCount;
        }

        public int ScopesProcessedCount { get; }
        public int GlobalRegistrationCount { get; }
        public int ProxySwapTargetsCount { get; }
        public int InjectedFieldCount { get; }
        public int InjectedPropertyCount { get; }
        public int InjectedMethodCount { get; }
        public int MissingDependencyCount { get; }
        public int MissingBindingCount { get; }
        public int InvalidBindingCount { get; }
        public int UnusedBindingCount { get; }
        public int SuppressedErrorCount { get; }

        private IReadOnlyCollection<InjectionError> Errors { get; }

        public LogSeverity GetLogSeverity()
        {
            if (Errors.Count(e => !e.SuppressError) > 0)
                return LogSeverity.Error;

            return UnusedBindingCount > 0 ? LogSeverity.Warning : LogSeverity.Info;
        }
    }
}