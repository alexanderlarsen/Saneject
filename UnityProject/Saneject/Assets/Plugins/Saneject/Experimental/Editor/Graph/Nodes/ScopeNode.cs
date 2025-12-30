using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Runtime;
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

        private static ScopeNode FindParentScopeNode(TransformNode transformNode)
        {
            TransformNode currentTransformNode = transformNode.ParentTransformNode;
            ScopeNode parentScope = null;

            while (currentTransformNode != null)
            {
                if (currentTransformNode.ScopeNode == null || (UserSettings.UseContextIsolation && currentTransformNode.ContextNode != transformNode.ContextNode))
                {
                    currentTransformNode = currentTransformNode.ParentTransformNode;
                    continue;
                }

                parentScope = currentTransformNode.ScopeNode;
                break;
            }

            return parentScope;
        }
    }
}