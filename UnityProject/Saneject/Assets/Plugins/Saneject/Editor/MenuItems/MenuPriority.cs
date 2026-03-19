using System.ComponentModel;

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class MenuPriority
    {
        public const int Root = -10000;
        public const int ComponentRoot = -10000;
        public const int Group = 20;
    }
}