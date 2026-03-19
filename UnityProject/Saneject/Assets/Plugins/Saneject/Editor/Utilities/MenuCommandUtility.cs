using System.ComponentModel;
using UnityEditor;

namespace Plugins.Saneject.Editor.Utilities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
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