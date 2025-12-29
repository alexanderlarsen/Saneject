using Plugins.Saneject.Experimental.Editor.Core;
using Plugins.Saneject.Experimental.Editor.Graph;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class BuildGraphMenuItems
    {
        [MenuItem("GameObject/Build Injection Graph And Save JSON", false, 49)]
        private static void BuildGraph()
        {
            const string path = @"E:\Unity\Personal\Saneject\UnityProject\Saneject\Assets\Plugins\Saneject\Experimental\GraphSystem\graph.json";

            InjectionGraph graph = GraphFactory.CreateInjectionGraph(Selection.gameObjects);
            InjectionGraphJsonExporter.SaveGraphToJson(graph, path);
        }
    }
}