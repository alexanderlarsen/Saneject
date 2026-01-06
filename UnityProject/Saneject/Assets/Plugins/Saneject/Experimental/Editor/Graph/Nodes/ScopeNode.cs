using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Experimental.Editor.Graph.Nodes
{
    public class ScopeNode
    {
        public ScopeNode(
            Scope scope,
            TransformNode transformNode)
        {
            TransformNode = transformNode;
            ParentScopeNode = FindParentScopeNode(transformNode);
            Type = scope.GetType();

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

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScopeNode { get; }
        public Type Type { get; }
        public IReadOnlyCollection<BindingNode> BindingNodes { get; }
        public Scope Scope { get; }

        private static ScopeNode FindParentScopeNode(TransformNode transformNode)
        {
            TransformNode currentTransformNode = transformNode.ParentTransformNode;
            ScopeNode parentScope = null;

            while (currentTransformNode != null)
            {
                if (currentTransformNode.DeclaredScopeNode == null || (UserSettings.UseContextIsolation && !currentTransformNode.ContextIdentity.Equals(transformNode.ContextIdentity)))
                {
                    currentTransformNode = currentTransformNode.ParentTransformNode;
                    continue;
                }

                parentScope = currentTransformNode.DeclaredScopeNode;
                break;
            }

            return parentScope;
        }
    }
}