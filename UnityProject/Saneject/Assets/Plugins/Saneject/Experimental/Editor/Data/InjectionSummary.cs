using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class InjectionSummary
    {
        public InjectionSummary(InjectionContext context)
        {
            GlobalRegistrationCount = context.ScopeGlobalResolutionMap.Count;
            InjectedFieldCount = context.FieldResolutionMap.Keys.Count(field => !field.IsPropertyBackingField);
            InjectedPropertyCount = context.FieldResolutionMap.Keys.Count(field => field.IsPropertyBackingField);
            InjectedMethodCount = context.MethodResolutionMap.Count;
            MissingBindingCount = context.Errors.Count(error => error.ErrorType == ErrorType.MissingBinding);
            UnusedBindingCount = context.UnusedBindings.Count;
            InvalidBindingCount = context.Errors.Count(error => error.ErrorType == ErrorType.InvalidBinding);
            MissingDependencyCount = context.Errors.Count(error => error.ErrorType is ErrorType.MissingDependency or ErrorType.MissingDependencies);
            SuppressedMissingCount = 2; // TODO: implement

            ScopesProcessedCount = context.Graph
                .EnumerateAllTransformNodes()
                .Count(transformNode => transformNode.DeclaredScopeNode != null);

            ElapsedMilliseconds = context.ElapsedMilliseconds;
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
        public int SuppressedMissingCount { get; }
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