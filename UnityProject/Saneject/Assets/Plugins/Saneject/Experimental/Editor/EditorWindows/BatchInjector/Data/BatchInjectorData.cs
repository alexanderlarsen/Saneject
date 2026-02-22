using System;
using Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Enums;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [Serializable]
    public class BatchInjectorData
    {
        public AssetList sceneList = new();
        public AssetList prefabList = new();
        public WindowTab windowTab = WindowTab.Scenes;

        [NonSerialized]
        public bool isDirty;
    }
}