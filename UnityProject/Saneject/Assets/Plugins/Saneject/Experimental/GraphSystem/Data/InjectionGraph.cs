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
        public InjectionGraph(params Transform[] startTransforms)
        {
            HashSet<Transform> processedRoots = new();
            
            RootNodes = startTransforms
                .Where(startTransform => processedRoots.Add(startTransform.root))
                .Select(transform => new TransformNode(transform.root))
                .ToList();
        }
        
        public IReadOnlyList<TransformNode> RootNodes { get; } 
    }
}