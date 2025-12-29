using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class DependencyResolver
    {
        public static void Resolve(InjectionGraph graph, out InjectionPlan sourceTargetMap, out IReadOnlyList<DependencyError> dependencyErrors)
        {
            sourceTargetMap = null;
            dependencyErrors = null;
        }
    }
}