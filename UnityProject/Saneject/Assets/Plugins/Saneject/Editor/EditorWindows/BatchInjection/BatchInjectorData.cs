using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList sceneList = new();
        public AssetList prefabList = new();
        public WindowTab windowTab = WindowTab.Scenes;

        public void Initialize()
        {
            sceneList.UpdateEnabledCount();
            prefabList.UpdateEnabledCount();
        }
    }
}