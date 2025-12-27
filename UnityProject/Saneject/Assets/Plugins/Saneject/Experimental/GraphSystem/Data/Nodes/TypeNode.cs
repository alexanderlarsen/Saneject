using System;
using Newtonsoft.Json;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    public class TypeNode
    {
        [JsonIgnore]
        public Type Type { get; set; }

        public string Name => Type.FullName;
    }
}