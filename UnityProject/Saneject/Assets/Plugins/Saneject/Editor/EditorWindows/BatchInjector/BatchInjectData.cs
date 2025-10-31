using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjector
{
    [Serializable]
    public class BatchInjectData
    {
        public List<AssetEntry> scenes = new();
        public List<AssetEntry> prefabs = new();
    }
}