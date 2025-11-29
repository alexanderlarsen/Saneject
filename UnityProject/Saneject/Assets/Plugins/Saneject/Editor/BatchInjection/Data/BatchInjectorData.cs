using System;
using Plugins.Saneject.Editor.BatchInjection.Enums;

namespace Plugins.Saneject.Editor.BatchInjection.Data
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList sceneList = new();
        public AssetList prefabList = new();
        public WindowTab windowTab = WindowTab.Scenes;
    }
}