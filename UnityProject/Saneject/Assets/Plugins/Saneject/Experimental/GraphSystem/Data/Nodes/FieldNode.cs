using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    [Serializable]
    public class FieldNode
    {
        [JsonIgnore]
        public FieldInfo FieldInfo { get; set; }

        [JsonProperty]
        private string Name => FieldInfo.Name;

        public string InjectId { get; set; }
        public bool SuppressMissingErrors { get; set; }
        public bool IsCollection { get; set; }
    }
}