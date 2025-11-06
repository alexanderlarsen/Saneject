using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList scenes = new();
        public AssetList prefabs = new();

        public SelectedTab selectedTab = SelectedTab.Scenes;

        public void Initialize()
        {
            scenes.UpdateEnabledCount();
            prefabs.UpdateEnabledCount();
        }
    }
}