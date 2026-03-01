using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [EditorBrowsable(EditorBrowsableState.Never), Serializable]
    public class PrefabAssetData : AssetData
    {
        public PrefabAssetData(string guid) : base(guid)
        {
        }
    }
}