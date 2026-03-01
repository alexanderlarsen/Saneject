using System;
using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Editor.EditorWindows.BatchInjector.Data
{
    [EditorBrowsable(EditorBrowsableState.Never), Serializable]
    public class SceneAssetData : AssetData
    {
        public SceneAssetData(string guid) : base(guid)
        {
        }
    }
}