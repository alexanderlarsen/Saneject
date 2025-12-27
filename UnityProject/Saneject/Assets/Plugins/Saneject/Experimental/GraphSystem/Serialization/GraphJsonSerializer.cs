using Newtonsoft.Json;
using Plugins.Saneject.Experimental.GraphSystem.Data;

namespace Plugins.Saneject.Experimental.GraphSystem.Serialization
{
    public static class GraphJsonSerializer
    {
        public static string Serialize(Graph graph)
        {
            return JsonConvert.SerializeObject
            (
                graph,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                }
            );
        }
    }
}