using UnityEditor;

namespace Plugins.Saneject.Experimental.Editor.Utilities
{
    public static class MenuCommandUtility
    {
        public static bool IsFirstInvocation(MenuCommand cmd)
        {
            if (Selection.objects.Length <= 1)
                return true;

            return cmd.context == Selection.objects[0];
        }
    }
}