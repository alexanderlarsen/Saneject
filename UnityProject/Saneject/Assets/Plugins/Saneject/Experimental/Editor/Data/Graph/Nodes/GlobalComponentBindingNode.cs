using System.ComponentModel;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;

namespace Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class GlobalComponentBindingNode : ComponentBindingNode
    {
        public GlobalComponentBindingNode(
            GlobalComponentBinding binding,
            ScopeNode scopeNode) : base(binding, scopeNode)
        {
        }
    }
}