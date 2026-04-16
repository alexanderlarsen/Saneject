using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugins.Saneject.Editor.Data.Graph.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Editor.Data.Graph
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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