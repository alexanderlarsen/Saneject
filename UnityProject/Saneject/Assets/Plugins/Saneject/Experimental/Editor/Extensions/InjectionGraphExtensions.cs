using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.BindingNodes;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class InjectionGraphExtensions
    {
        public static IEnumerable<TransformNode> GetAllTransformNodes(this InjectionGraph graph)
        {
            return graph.RootNodes.SelectMany(GetTransformNodesRecursive);

            IEnumerable<TransformNode> GetTransformNodesRecursive(TransformNode node)
            {
                yield return node;

                foreach (TransformNode child in node.Children.SelectMany(GetTransformNodesRecursive))
                    yield return child;
            }
        }

        public static IEnumerable<ScopeNode> GetAllScopeNodes(this InjectionGraph graph)
        {
            return graph.RootNodes.SelectMany(GetScopeNodesRecursive);

            IEnumerable<ScopeNode> GetScopeNodesRecursive(TransformNode node)
            {
                if (node.Scope != null)
                    yield return node.Scope;

                foreach (ScopeNode scope in node.Children.SelectMany(GetScopeNodesRecursive))
                    yield return scope;
            }
        }

        public static IEnumerable<BaseBindingNode> GetAllBindings(this ScopeNode scopeNode)
        {
            foreach (ComponentBindingNode componentBinding in scopeNode.ComponentBindings)
                yield return componentBinding;

            foreach (AssetBindingNode assetBinding in scopeNode.AssetBindings)
                yield return assetBinding;

            foreach (GlobalComponentBindingNode globalBinding in scopeNode.GlobalBindings)
                yield return globalBinding;
        }

        public static IEnumerable<BindingContext> GetAllBindingContexts(this InjectionGraph graph)
        {
            foreach (TransformNode transform in graph.GetAllTransformNodes())
            {
                if (transform.Scope == null)
                    continue;

                foreach (BaseBindingNode binding in transform.Scope.GetAllBindings())
                    yield return new BindingContext(binding, transform.Scope);
            }
        }
    }
}