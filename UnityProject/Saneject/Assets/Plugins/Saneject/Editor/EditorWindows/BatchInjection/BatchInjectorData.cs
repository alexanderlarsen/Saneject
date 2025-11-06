using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class BatchInjectorData
    {
        public List<AssetData> scenes = new();
        public List<AssetData> prefabs = new();

        public SortMode sceneSortMode = SortMode.NameAtoZ;
        public SortMode prefabSortMode = SortMode.NameAtoZ;
        public SelectedTab selectedTab = SelectedTab.Scenes;
    }
}