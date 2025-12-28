using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;
using UnityEngine;

namespace Plugins.Saneject.Experimental.GraphSystem.Data
{
    [Serializable]
    public class InjectionGraph
    {
        private InjectionGraph(params Transform[] startTransforms)
        {
            RootNodes = startTransforms
                .Select(transform => transform.root)
                .Distinct()
                .Select(root => new TransformNode(root))
                .ToList();
        }

        public IReadOnlyList<TransformNode> RootNodes { get; }

        public static InjectionGraph Build(params Transform[] startTransforms)
        {
            return new InjectionGraph(startTransforms);
        }

        public static InjectionGraph Build(params GameObject[] startGameObjects)
        {
            return new InjectionGraph(startGameObjects.Select(go => go.transform).ToArray());
        }
    }
}