using Plugins.Saneject.Experimental.Editor.Core;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectionMenuItems
    {
        [MenuItem("GameObject/Inject Hierarchy (new)", false, 49)]
        private static void InjectHierarchy()
        {
            PipelineRunner.InjectSingleHierarchy(Selection.activeGameObject);
        }
    }
}