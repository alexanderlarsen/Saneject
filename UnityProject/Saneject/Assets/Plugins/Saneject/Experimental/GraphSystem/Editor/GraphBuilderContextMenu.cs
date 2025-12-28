using Plugins.Saneject.Experimental.GraphSystem.Data;
using Plugins.Saneject.Experimental.GraphSystem.Debugging;
using UnityEditor;

namespace Plugins.Saneject.Experimental.GraphSystem.Editor
{
    public static class GraphBuilderContextMenu
    {
        [MenuItem("GameObject/Build Injection Graph And Save JSON", false, 49)]
        private static void BuildGraph()
        {
            const string path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\GraphSystem\graph.json";

            InjectionGraph graph = InjectionGraph.Build(Selection.gameObjects);
            InjectionGraphJsonExporter.SaveGraphToJson(graph, path);
        }

        [MenuItem("GameObject/Build Injection Graph And Save JSON", true, 49)]
        private static bool BuildGraphValidate()
        {
            return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
        }
    }
}