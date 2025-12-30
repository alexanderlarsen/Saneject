using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    }
}