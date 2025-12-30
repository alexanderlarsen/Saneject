using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;
using Plugins.Saneject.Experimental.Runtime;

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class ScopeNode
    {
        public ScopeNode(
            Scope scope,
            TransformNode transformNode)
        {
            TransformNode = transformNode;
            ParentScopeNode = transformNode.FindParentScopeNode();
            Type = scope.GetType();
            scope.ConfigureBindings();

            ComponentBindingNodes = scope.ComponentBindings
                .Select(binding => new ComponentBindingNode(binding, this))
                .ToList();

            AssetBindingNodes = scope.AssetBindings
                .Select(binding => new AssetBindingNode(binding, this))
                .ToList();

            GlobalComponentBindingNodes = scope.GlobalBindings
                .Select(binding => new GlobalComponentBindingNode(binding, this))
                .ToList();
        }

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScopeNode { get; }
        public Type Type { get; }
        public IReadOnlyCollection<ComponentBindingNode> ComponentBindingNodes { get; }
        public IReadOnlyCollection<AssetBindingNode> AssetBindingNodes { get; }
        public IReadOnlyCollection<GlobalComponentBindingNode> GlobalComponentBindingNodes { get; }
    }
}