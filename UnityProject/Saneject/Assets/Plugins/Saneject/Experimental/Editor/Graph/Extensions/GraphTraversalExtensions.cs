using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Graph.Extensions
{
    public static class GraphTraversalExtensions
    {
        public static IEnumerable<TransformNode> EnumerateAllTransformNodes(this InjectionGraph graph)
        {
            return graph.RootTransformNodes.SelectMany(GetTransformNodesRecursive);

            IEnumerable<TransformNode> GetTransformNodesRecursive(TransformNode node)
            {
                yield return node;

                foreach (TransformNode child in node.ChildTransformNodes.SelectMany(GetTransformNodesRecursive))
                    yield return child;
            }
        }

        public static IEnumerable<BindingNode> EnumerateAllBindingNodes(this InjectionGraph graph)
        {
            foreach (TransformNode transform in graph.EnumerateAllTransformNodes())
            {
                if (transform.ScopeNode == null)
                    continue;

                foreach (BindingNode binding in transform.ScopeNode.EnumerateAllBindingNodes())
                    yield return binding;
            }
        }

        public static IEnumerable<BindingNode> EnumerateAllBindingNodes(this ScopeNode scopeNode)
        {
            foreach (ComponentBindingNode componentBinding in scopeNode.ComponentBindingNodes)
                yield return componentBinding;

            foreach (AssetBindingNode assetBinding in scopeNode.AssetBindingNodes)
                yield return assetBinding;

            foreach (GlobalComponentBindingNode globalBinding in scopeNode.GlobalComponentBindingNodes)
                yield return globalBinding;
        }
    }
}