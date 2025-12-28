using System;
using System.Collections.Generic;
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    public class ScopeNode
    {
        public ScopeNode(
            Scope scope,
            TransformNode transformNode)
        {
            TransformNode = transformNode;
            ParentScope = FindParentScopeNode(transformNode);
            Type = scope.GetType();
            Bindings = new List<BindingNode>();
        }

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScope { get; }
        public Type Type { get; }
        public IReadOnlyList<BindingNode> Bindings { get; }

        private static ScopeNode FindParentScopeNode(TransformNode transformNode)
        {
            TransformNode current = transformNode.Parent;
            ScopeNode parentScope = null;

            while (current != null)
            {
                if (current.Scope == null || (UserSettings.UseContextIsolation && current.Context != transformNode.Context))
                {
                    current = current.Parent;
                    continue;
                }

                parentScope = current.Scope;
                break;
            }

            return parentScope;
        }
    }
}