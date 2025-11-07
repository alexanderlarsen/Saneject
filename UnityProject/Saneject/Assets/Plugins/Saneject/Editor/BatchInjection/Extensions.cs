using Plugins.Saneject.Editor.Core;

namespace Plugins.Saneject.Editor.BatchInjection
{
    public static class Extensions
    {
        public static InjectionStatus GetInjectionStatus(this InjectionStats stats)
        {
            int totalErrors = stats.numMissingBindings + stats.numInvalidBindings + stats.numMissingDependencies;

            if (totalErrors > 0)
                return InjectionStatus.Error;

            return stats.numUnusedBindings > 0 ? InjectionStatus.Warning : InjectionStatus.Success;
        }
    }
}