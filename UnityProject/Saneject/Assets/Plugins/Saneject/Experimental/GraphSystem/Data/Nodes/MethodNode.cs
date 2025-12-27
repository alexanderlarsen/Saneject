using System.Reflection;
using Newtonsoft.Json;

namespace Plugins.Saneject.Experimental.GraphSystem.Data.Nodes
{
    public class MethodNode
    {
        [JsonIgnore]
        private MethodInfo MethodInfo { get; set; }

        [JsonProperty]
        private string Name => MethodInfo.Name;

        public string InjectId { get; set; }
        public bool SuppressMissingErrors { get; set; }
    }
}