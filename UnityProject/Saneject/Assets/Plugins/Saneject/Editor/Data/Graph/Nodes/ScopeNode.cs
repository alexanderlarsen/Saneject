using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Extensions;
using Plugins.Saneject.Runtime.Bindings.Asset;
using Plugins.Saneject.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Scopes;

namespace Plugins.Saneject.Editor.Data.Graph.Nodes
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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
        public IReadOnlyList<BindingNode> BindingNodes { get; }
    }
}