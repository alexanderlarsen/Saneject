using System.ComponentModel;
using Plugins.Saneject.Runtime.Bindings.Component;

namespace Plugins.Saneject.Editor.Data.Graph.Nodes
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