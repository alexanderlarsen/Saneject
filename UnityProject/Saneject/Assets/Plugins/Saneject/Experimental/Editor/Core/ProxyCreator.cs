using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Core
{
    public static class ProxyCreator
    {
        public static void CreateAllProxies(
            InjectionSession session,
            out int proxiesCreated)
        {
            proxiesCreated = 0;
        }

        public static bool ShouldCreateProxies(InjectionSession session)
        {
            return session
                .Graph
                .EnumerateAllBindingNodes()
                .Count(binding => binding is ComponentBindingNode { ResolveFromProxy: true }) > 0;
        }
    }
}