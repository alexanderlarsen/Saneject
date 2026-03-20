using System.ComponentModel;

namespace Plugins.Saneject.Editor.MenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class MenuPriority
    {
        public static class SanejectMenu
        {
            public const int Base = -10000;

            public static class CreateNewScope
            {
                public const int LocalBase = Base + 0;
                public const int Item = LocalBase + 1;
            }

            public static class InjectScene
            {
                public const int LocalBase = Base + 100;
                public const int AllContexts = LocalBase + 1;
                public const int SceneObjects = LocalBase + 2;
                public const int PrefabInstances = LocalBase + 3;
            }

            public static class InjectSelectedSceneHierarchies
            {
                public const int LocalBase = Base + 105;
                public const int AllContexts = LocalBase + 1;
                public const int SceneObjects = LocalBase + 2;
                public const int PrefabInstances = LocalBase + 3;
                public const int SameContextsAsSelection = LocalBase + 4;
            }

            public static class InjectPrefabAsset
            {
                public const int LocalBase = Base + 110;
                public const int AllContexts = LocalBase + 1;
                public const int PrefabAssetObjects = LocalBase + 2;
                public const int PrefabInstances = LocalBase + 3;
                public const int SameContextsAsSelection = LocalBase + 4;
            }

            public static class BatchInject
            {
                public const int LocalBase = Base + 115;
                public const int SelectedAssetsAllContexts = LocalBase + 1;
            }

            public static class SelectSameContextObjects
            {
                public const int LocalBase = Base + 200;
                public const int InScene = LocalBase + 1;
                public const int InHierarchy = LocalBase + 2;
            }

            public static class RuntimeProxy
            {
                public const int LocalBase = Base + 300;
                public const int GenerateMissingProxyScripts = LocalBase + 1;
                public const int CleanUpUnusedScriptsAndAssets = LocalBase + 2;
            }

            public static class Settings
            {
                public const int LocalBase = Base + 400;
                public const int Show = LocalBase + 1;
            }
        }

        public static class ComponentMenu
        {
            public const int Base = -10000;

            public static class Filter
            {
                public const int LocalBase = Base + 0;
                public const int LogsByScopeType = LocalBase + 1;
                public const int LogsByComponentPath = LocalBase + 2;
            }
        }
    }
}
