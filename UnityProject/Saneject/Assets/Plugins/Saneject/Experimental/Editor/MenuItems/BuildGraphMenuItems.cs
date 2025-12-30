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
            InjectionGraphJsonExporter.SaveGraphToJson
            (
                InjectionGraphFactory.CreateGraph(Selection.gameObjects)
            );
        }
    }
}