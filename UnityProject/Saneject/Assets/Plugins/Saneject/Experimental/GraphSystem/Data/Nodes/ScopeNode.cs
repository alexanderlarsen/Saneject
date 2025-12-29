using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.GraphSystem.Bindings;
using Plugins.Saneject.Runtime.Settings;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    public class ScopeNode
    {
        public ScopeNode(
            NewScope scope,
            TransformNode transformNode)
        {
            TransformNode = transformNode;
            ParentScope = FindParentScopeNode(transformNode);
            Type = scope.GetType();
            scope.ConfigureBindings();
            ComponentBindings = scope.ComponentBindings;
            AssetBindings = scope.AssetBindings;
            GlobalBindings = scope.GlobalBindings;
        }

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScope { get; }
        public Type Type { get; }
        public IReadOnlyCollection<NewComponentBinding> ComponentBindings { get; }
        public IReadOnlyCollection<NewAssetBinding> AssetBindings { get; }
        public IReadOnlyCollection<NewComponentBinding> GlobalBindings { get; }

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