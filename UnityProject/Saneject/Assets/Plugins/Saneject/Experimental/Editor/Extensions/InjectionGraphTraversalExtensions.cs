using System.Collections.Generic;
using Plugins.Saneject.Experimental.Editor.Graph;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;

namespace Plugins.Saneject.Experimental.Editor.Extensions
{
    public static class InjectionGraphTraversalExtensions
    {
        public static IEnumerable<TransformNode> GetAllTransformNodes(this InjectionGraph graph)
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

        public static IEnumerable<BindingNode> GetAllBindingNodes(this InjectionGraph graph)
        {
            foreach (TransformNode transformNode in graph.GetAllTransformNodes())
            {
                if (transformNode.DeclaredScopeNode == null)
                    continue;

                foreach (BindingNode binding in transformNode.DeclaredScopeNode.BindingNodes)
                    yield return binding;
            }
        }
    }
}