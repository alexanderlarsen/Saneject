using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.Runtime;
using Plugins.Saneject.Experimental.Runtime.Bindings;
using Plugins.Saneject.Experimental.Runtime.Bindings.Asset;
using Plugins.Saneject.Experimental.Runtime.Bindings.Component;
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
            ComponentBindings = scope.ComponentBindings;
            AssetBindings = scope.AssetBindings;
            GlobalBindings = scope.GlobalBindings;
        }

        public TransformNode TransformNode { get; }
        public ScopeNode ParentScope { get; }
        public Type Type { get; }
        public IReadOnlyCollection<ComponentBinding> ComponentBindings { get; }
        public IReadOnlyCollection<AssetBinding> AssetBindings { get; }
        public IReadOnlyCollection<ComponentBinding> GlobalBindings { get; }

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