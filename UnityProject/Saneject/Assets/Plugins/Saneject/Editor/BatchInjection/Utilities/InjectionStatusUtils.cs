using Plugins.Saneject.Editor.BatchInjection.Enums;
using Plugins.Saneject.Editor.Core;

namespace Plugins.Saneject.Editor.BatchInjection.Utilities
{
    public static class InjectionStatusUtils
    {
        public static InjectionStatus GetInjectionStatusFromStats(InjectionStats stats)
        {
            int totalErrors = stats.numMissingBindings + stats.numInvalidBindings + stats.numMissingDependencies;

            if (totalErrors > 0)
                return InjectionStatus.Error;

            return stats.numUnusedBindings > 0 ? InjectionStatus.Warning : InjectionStatus.Success;
        }
    }
}