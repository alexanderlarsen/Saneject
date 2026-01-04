using Plugins.Saneject.Experimental.Editor.Core;
using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.MenuItems
{
    public static class InjectionMenuItems
    {
        [MenuItem("GameObject/Inject From Selection (NEW PIPELINE)", false, 49), MenuItem("Assets/Inject From Selection (NEW PIPELINE)", false, 49)]
        private static void InjectFromSelection()
        {
            InjectionPipeline.Inject(Selection.gameObjects);
        }
    }
}