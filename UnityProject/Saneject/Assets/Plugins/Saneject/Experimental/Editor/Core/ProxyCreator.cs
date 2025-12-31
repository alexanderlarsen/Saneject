using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class ProxyCreator
    {
        public static void CreateAllProxies(InjectionGraph graph, out int proxiesCreated)
        {
            proxiesCreated = 0;
        }

        public static bool ShouldCreateProxies(InjectionGraph graph)
        {
            return graph
                .EnumerateAllBindingNodes()
                .Count(binding => binding is ComponentBindingNode { ResolveFromProxy: true }) > 0;
        }
    }
}