using System.ComponentModel;

namespace Plugins.Saneject.Runtime.Bindings.Asset
{
    /// <summary>
    /// Specifies how an asset should be loaded.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum AssetLoadType
    {
        /// <summary> Load a single asset from the Resources folder. </summary>
        Resources,
        /// <summary> Load all assets from a Resources folder path. </summary>
        ResourcesAll,
        /// <summary> Load a single asset from a specific path. </summary>
        AssetLoad, 
        /// <summary> Load all assets within a specific folder. </summary>
        Folder,
        /// <summary> Use a direct object instance. </summary>
        Instance 
    }
}