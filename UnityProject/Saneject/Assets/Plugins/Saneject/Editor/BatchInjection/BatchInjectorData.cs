using System;

namespace Plugins.Saneject.Editor.BatchInjection
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