using System;
using System.Collections.Generic;
using Plugins.Saneject.Experimental.GraphSystem.Data.Nodes;

namespace Plugins.Saneject.Experimental.GraphSystem.Data
{
    [Serializable]
    public class Graph
    {
        public List<TransformNode> RootNodes { get; } = new();
    }
}