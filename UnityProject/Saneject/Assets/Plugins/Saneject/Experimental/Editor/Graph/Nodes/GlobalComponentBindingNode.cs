using Plugins.Saneject.Experimental.Runtime.Bindings.Component;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class GlobalComponentBindingNode : ComponentBindingNode
    {
        public GlobalComponentBindingNode(
            GlobalComponentBinding binding,
            ScopeNode scopeNode) : base(binding, scopeNode)
        {
        }
    }
}