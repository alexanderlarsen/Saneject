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
        public InjectionGraph(Transform[] startTransforms)
        {
            RootTransformNodes = startTransforms
                .Select(transform => transform.root)
                .Distinct()
                .Select(root => new TransformNode(root, startTransforms))
                .ToList();
        }

        public IReadOnlyCollection<TransformNode> RootTransformNodes { get; }
    }
}