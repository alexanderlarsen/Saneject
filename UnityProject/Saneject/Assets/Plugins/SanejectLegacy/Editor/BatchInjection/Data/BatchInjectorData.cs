using System;
using Plugins.SanejectLegacy.Editor.BatchInjection.Enums;

namespace Plugins.SanejectLegacy.Editor.BatchInjection.Data
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList sceneList = new();
        public AssetList prefabList = new();
        public WindowTab windowTab = WindowTab.Scenes;
    }
}