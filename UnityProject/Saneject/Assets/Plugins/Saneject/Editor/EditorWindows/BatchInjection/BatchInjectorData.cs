using System;
using System.Collections.Generic;

namespace Plugins.Saneject.Editor.EditorWindows.BatchInjection
{
    [Serializable]
    public class BatchInjectorData
    {
        public List<AssetData> scenes = new();
        public List<AssetData> prefabs = new();
    }
}