using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    [Serializable]
    public class TransformNode
    {
        [JsonIgnore]
        public TransformNode Parent { get; set; }

        #region Debug Properties

        [JsonProperty]
        private int DebugParentId => Parent?.Id ?? 0;

        #endregion

        public string Name { get; set; }
        public int Id { get; set; }
        public ContextNode Context { get; set; }
        public ScopeNode Scope { get; set; }
        public List<ComponentNode> Components { get; set; }
        public List<TransformNode> Children { get; set; }
    }
}