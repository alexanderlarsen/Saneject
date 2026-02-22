using System;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [Serializable]
    public class PrefabAssetData : AssetData
    {
        public PrefabAssetData(string guid) : base(guid)
        {
        }
    }
}