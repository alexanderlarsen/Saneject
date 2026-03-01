using System.ComponentModel;

namespace Plugins.Saneject.Experimental.Runtime.Bindings.Asset
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum AssetLoadType
    {
        Resources,
        ResourcesAll,
        AssetLoad,
        AssetLoadAll,
        Folder,
        Instance 
    }
}