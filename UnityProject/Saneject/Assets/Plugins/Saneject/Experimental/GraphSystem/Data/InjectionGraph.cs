using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Data
{
    public class InjectionGraph
    {
        public InjectionGraph(params Transform[] startTransforms)
        {
            RootNodes = startTransforms
                .Select(transform => transform.root)
                .Distinct()
                .Select(root => new TransformNode(root))
                .ToList();
        }

        public InjectionGraph(params GameObject[] startGameObjects)
            : this(startGameObjects.Select(go => go.transform).ToArray())
        {
        }

        public IReadOnlyList<TransformNode> RootNodes { get; }
    }
}