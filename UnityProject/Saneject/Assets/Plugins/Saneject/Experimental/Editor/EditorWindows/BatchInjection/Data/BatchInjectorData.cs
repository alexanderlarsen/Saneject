using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Enums;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjection.Data
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList sceneList = new();
        public AssetList prefabList = new();
        public WindowTab windowTab = WindowTab.Scenes;
    }
}