using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    [Serializable]
    public class ComponentNode
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public List<FieldNode> Properties { get; set; }
        public List<MethodNode> Methods { get; set; }
    }
}