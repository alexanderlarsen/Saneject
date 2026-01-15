using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

// ReSharper disable LoopCanBePartlyConvertedToQuery
// ReSharper disable once LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class InjectionGraphTraversalExtensions
    {
        public static IEnumerable<TransformNode> EnumerateAllTransformNodes(this InjectionGraph graph)
        {
            foreach (TransformNode root in graph.RootTransformNodes)
                foreach (TransformNode node in EnumerateTransformNodesRecursive(root))
                    yield return node;

            yield break;

            static IEnumerable<TransformNode> EnumerateTransformNodesRecursive(TransformNode node)
            {
                yield return node;

                foreach (TransformNode child in node.ChildTransformNodes)
                    foreach (TransformNode descendant in EnumerateTransformNodesRecursive(child))
                        yield return descendant;
            }
        }

        public static IEnumerable<ComponentNode> EnumerateAllComponentNodes(this IEnumerable<TransformNode> transformNodes)
        {
            foreach (TransformNode transformNode in transformNodes)
                foreach (ComponentNode componentNode in transformNode.ComponentNodes)
                    yield return componentNode;
        }

        public static IEnumerable<ScopeNode> EnumerateAllScopeNodes(this IEnumerable<TransformNode> transformNodes)
        {
            foreach (TransformNode transformNode in transformNodes)
                if (transformNode.DeclaredScopeNode != null)
                    yield return transformNode.DeclaredScopeNode;
        }

        public static IEnumerable<BindingNode> EnumerateAllBindingNodes(this IEnumerable<ScopeNode> scopeNodes)
        {
            foreach (ScopeNode scopeNode in scopeNodes)
                foreach (BindingNode bindingNode in scopeNode.BindingNodes)
                    yield return bindingNode;
        }
    }
}