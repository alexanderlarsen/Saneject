using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Extensions;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Experimental.Runtime.Scopes;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class ScopeNode
    {
        public ScopeNode(
            Scope scope,
            TransformNode transformNode)
        {
            TransformNode = transformNode;
            ParentScopeNode = transformNode.ParentTransformNode.FindNearestScope();
            ScopeType = scope.GetType();

            BindingNodes = scope
                .CollectBindings()
                .Select(binding => binding switch
                {
                    GlobalComponentBinding globalComponentBinding => new GlobalComponentBindingNode(globalComponentBinding, this),
                    ComponentBinding componentBinding => new ComponentBindingNode(componentBinding, this),
                    AssetBinding assetBinding => new AssetBindingNode(assetBinding, this) as BindingNode,
                    _ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
                })
                .ToList();

            Scope = scope;
        }

        public Scope Scope { get; }
        public TransformNode TransformNode { get; }
        public ScopeNode ParentScopeNode { get; }
        public Type ScopeType { get; }
        public IReadOnlyCollection<BindingNode> BindingNodes { get; }
    }
}