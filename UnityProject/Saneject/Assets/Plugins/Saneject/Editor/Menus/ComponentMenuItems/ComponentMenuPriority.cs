using System.ComponentModel;

namespace Plugins.Saneject.Editor.Menus.ComponentMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ComponentMenuPriority
    {
        public const int Root = -10000;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static class Filter
        {
            public const int Base = Root + 0;
            public const int LogsByScopeType = Base + 1;
            public const int LogsByComponentPath = Base + 2;
        }
    }
}