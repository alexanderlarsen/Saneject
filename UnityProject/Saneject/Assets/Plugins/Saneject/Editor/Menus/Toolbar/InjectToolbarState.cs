namespace Plugins.Saneject.Editor.Menus.Toolbar
{
    public readonly struct InjectToolbarState
    {
        private InjectToolbarState(
            bool injectSceneButtonEnabled,
            bool injectHierarchiesButtonEnabled,
            bool injectPrefabButtonEnabled,
            bool batchInjectButtonEnabled)
        {
            InjectSceneButtonEnabled = injectSceneButtonEnabled;
            InjectHierarchiesButtonEnabled = injectHierarchiesButtonEnabled;
            InjectPrefabButtonEnabled = injectPrefabButtonEnabled;
            BatchInjectButtonEnabled = batchInjectButtonEnabled;
        }

        public bool InjectSceneButtonEnabled { get; }
        public bool InjectHierarchiesButtonEnabled { get; }
        public bool InjectPrefabButtonEnabled { get; }
        public bool BatchInjectButtonEnabled { get; }

        public static InjectToolbarState Get()
        {
            bool isPrefab = MenuValidator.IsPrefabStage();
            bool isScene = MenuValidator.IsScene();
            int sceneObjectSelectionCount = MenuValidator.GetSceneObjectSelectionCount();
            bool hasBatchInjectAssetSelection = MenuValidator.HasValidBatchSelection();

            return new InjectToolbarState
            (
                injectSceneButtonEnabled: isScene && !isPrefab,
                injectHierarchiesButtonEnabled: isScene && !isPrefab && sceneObjectSelectionCount > 0,
                injectPrefabButtonEnabled: isPrefab && !isScene,
                batchInjectButtonEnabled: hasBatchInjectAssetSelection
            );
        }
    }
}