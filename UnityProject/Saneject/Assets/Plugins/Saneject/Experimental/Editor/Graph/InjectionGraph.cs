using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Graph.Nodes;
using UnityEngine;

// ReSharper disable LoopCanBePartlyConvertedToQuery
// ReSharper disable LoopCanBeConvertedToQuery

namespace Plugins.Saneject.Experimental.Editor.Graph
{
    public class InjectionGraph
    {
        public InjectionGraph(IEnumerable<Transform> startTransforms)
        {
            RootTransformNodes = startTransforms
                .Select(transform => transform.root)
                .Distinct()
                .Select(root => new TransformNode(root))
                .ToList();
        }

        public IReadOnlyList<TransformNode> RootTransformNodes { get; }

        public IEnumerable<TransformNode> EnumerateAllTransformNodes()
        {
            foreach (TransformNode root in RootTransformNodes)
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

        public IEnumerable<BindingNode> EnumerateAllBindingNodes()
        {
            foreach (TransformNode transformNode in EnumerateAllTransformNodes())
            {
                if (transformNode.DeclaredScopeNode == null)
                    continue;

                foreach (BindingNode binding in transformNode.DeclaredScopeNode.BindingNodes)
                    yield return binding;
            }
        }
    }
}