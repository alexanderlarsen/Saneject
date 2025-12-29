using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data
{
    public class BindingContext
    {
        public BindingContext(
            BaseBindingNode binding,
            ScopeNode scope)
        {
            Binding = binding;
            Scope = scope;
        }

        public BaseBindingNode Binding { get; }
        public ScopeNode Scope { get; } 
    }
}