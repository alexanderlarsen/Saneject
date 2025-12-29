using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Experimental.Editor.Graph
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
            scope.ConfigureBindings();

            ComponentBindings = scope.ComponentBindings
                .Select(binding => new ComponentBindingNode(binding))
                .ToList();

            AssetBindings = scope.AssetBindings
                .Select(binding => new AssetBindingNode(binding))
                .ToList();

            GlobalBindings = scope.GlobalBindings
                .Select(binding => new GlobalComponentBindingNode(binding))
                .ToList();
        }

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScope { get; }
        public Type Type { get; }
        public IReadOnlyCollection<ComponentBindingNode> ComponentBindings { get; }
        public IReadOnlyCollection<AssetBindingNode> AssetBindings { get; }
        public IReadOnlyCollection<GlobalComponentBindingNode> GlobalBindings { get; }

        private static ScopeNode FindParentScopeNode(TransformNode transformNode)
        {
            TransformNode currentTransformNode = transformNode.Parent;
            ScopeNode parentScope = null;

            while (currentTransformNode != null)
            {
                if (currentTransformNode.Scope == null || (UserSettings.UseContextIsolation && currentTransformNode.Context != transformNode.Context))
                {
                    currentTransformNode = currentTransformNode.Parent;
                    continue;
                }

                parentScope = currentTransformNode.Scope;
                break;
            }

            return parentScope;
        }
    }
}