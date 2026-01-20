using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.Editor.Data.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.Editor.Data.Graph
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

        public IReadOnlyCollection<TransformNode> RootTransformNodes { get; }
    }
}