using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    [Serializable]
    public class ScopeNode
    {
        [JsonIgnore]
        public ScopeNode ParentScope { get; set; }

        [JsonProperty]
        private int DebugParentScopeId => ParentScope?.Id ?? 0;

        public string Name { get; set; }
        public int Id { get; set; }

        public List<BindingNode> Bindings { get; set; }
    }
}