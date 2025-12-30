using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

// ReSharper disable LoopCanBePartlyConvertedToQuery
// ReSharper disable LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Graph.Extensions
{
    public static class GraphTraversalExtensions
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

        public static IEnumerable<BindingNode> EnumerateAllBindingNodes(this InjectionGraph graph)
        {
            foreach (TransformNode transformNode in graph.EnumerateAllTransformNodes())
            {
                if (transformNode.DeclaredScopeNode == null)
                    continue;

                foreach (BindingNode binding in transformNode.DeclaredScopeNode.BindingNodes)
                    yield return binding;
            }
        }

        public static IEnumerable<FieldNode> EnumerateAllFieldNodes(this InjectionGraph graph)
        {
            foreach (TransformNode transformNode in graph.EnumerateAllTransformNodes())
                foreach (ComponentNode componentNode in transformNode.ComponentNodes)
                    foreach (FieldNode fieldNode in componentNode.FieldNodes)
                        yield return fieldNode;
        }
    }
}