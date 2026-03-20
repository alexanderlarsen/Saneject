using System.ComponentModel;

namespace Plugins.Saneject.Editor.Menus.SanejectMenuItems
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SanejectMenuPriority
    {
        public const int Root = -10000;

        public static class CreateNewScope
        {
            public const int Base = Root + 0;
            public const int Item = Base + 1;
        }

        public static class InjectScene
        {
            public const int Base = Root + 100;
            public const int AllContexts = Base + 1;
            public const int SceneObjects = Base + 2;
            public const int PrefabInstances = Base + 3;
        }

        public static class InjectSelectedSceneHierarchies
        {
            public const int Base = Root + 105;
            public const int AllContexts = Base + 1;
            public const int SceneObjects = Base + 2;
            public const int PrefabInstances = Base + 3;
            public const int SameContextsAsSelection = Base + 4;
        }

        public static class InjectPrefabAsset
        {
            public const int Base = Root + 110;
            public const int AllContexts = Base + 1;
            public const int PrefabAssetObjects = Base + 2;
            public const int PrefabInstances = Base + 3;
            public const int SameContextsAsSelection = Base + 4;
        }

        public static class BatchInject
        {
            public const int Base = Root + 115;
            public const int SelectedAssetsAllContexts = Base + 1;
        }

        public static class SelectSameContextObjects
        {
            public const int Base = Root + 200;
            public const int InScene = Base + 1;
            public const int InHierarchy = Base + 2;
        }

        public static class RuntimeProxy
        {
            public const int Base = Root + 300;
            public const int GenerateMissingProxyScripts = Base + 1;
            public const int CleanUpUnusedScriptsAndAssets = Base + 2;
        }

        public static class Settings
        {
            public const int Base = Root + 400;
            public const int Show = Base + 1;
        }
    }
}